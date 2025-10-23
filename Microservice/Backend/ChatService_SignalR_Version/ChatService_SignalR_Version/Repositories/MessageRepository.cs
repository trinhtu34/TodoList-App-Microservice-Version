using ChatService_SignalR_Version.Models;
using Cassandra;
using ISession = Cassandra.ISession;

namespace ChatService_SignalR_Version.Repositories;

public interface IMessageRepository
{
    Task<ChatMessage> SaveAsync(ChatMessage message);
    Task<List<ChatMessage>> GetByGroupAsync(int groupId, int limit = 50);
    Task<ChatMessage?> GetByIdAsync(int groupId, Guid messageId);
}
public class MessageRepository : IMessageRepository
{
    private readonly ISession _session;
    private readonly ILogger<MessageRepository> _logger;

    public MessageRepository(ISession session, ILogger<MessageRepository> logger)
    {
        _session = session;
        _logger = logger;
    }

    public async Task<ChatMessage> SaveAsync(ChatMessage message)
    {
        // Generate TIMEUUID for message_id
        var messageId = TimeUuid.NewId();
        message.MessageId = messageId.ToGuid();
        message.CreatedAt = DateTime.UtcNow;

        var statement = new SimpleStatement(@"
            INSERT INTO messages (group_id, created_at, message_id, user_id, message_text, message_type, metadata, is_edited, is_deleted) 
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",        
            message.GroupId,
            message.CreatedAt,
            messageId,
            message.UserId,
            message.MessageText,
            message.MessageType,
            message.Metadata,
            message.IsEdited,
            message.IsDeleted);

        await _session.ExecuteAsync(statement);
        
        _logger.LogInformation($"Message saved: {message.MessageId} in group {message.GroupId}");
        return message;
    }

    public async Task<List<ChatMessage>> GetByGroupAsync(int groupId, int limit = 50)
    {
        var statement = new SimpleStatement(
            "SELECT * FROM messages WHERE group_id = ? ORDER BY created_at DESC, message_id DESC LIMIT ?",
            groupId, limit);

        var result = await _session.ExecuteAsync(statement);
        var messages = result.Select(MapRowToMessage).ToList();

        _logger.LogInformation($"Retrieved {messages.Count} messages for group {groupId}");
        return messages;
    }

    public async Task<ChatMessage?> GetByIdAsync(int groupId, Guid messageId)
    {
        var statement = new SimpleStatement(
            "SELECT * FROM messages WHERE group_id = ? AND message_id = ?",
            groupId, TimeUuid.Parse(messageId.ToString()));

        var result = await _session.ExecuteAsync(statement);
        var row = result.FirstOrDefault();

        return row != null ? MapRowToMessage(row) : null;
    }

    private ChatMessage MapRowToMessage(Row row)
    {
        return new ChatMessage
        {
            GroupId = row.GetValue<int>("group_id"),
            CreatedAt = row.GetValue<DateTime>("created_at"),
            MessageId = row.GetValue<TimeUuid>("message_id").ToGuid(),
            UserId = row.GetValue<string>("user_id"),
            MessageText = row.GetValue<string>("message_text"),
            MessageType = row.GetValue<string>("message_type"),
            Metadata = row.GetValue<IDictionary<string, string>>("metadata")?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new(),
            IsEdited = row.GetValue<bool>("is_edited"),
            IsDeleted = row.GetValue<bool>("is_deleted"),
            EditedAt = row.IsNull("edited_at") ? null : row.GetValue<DateTime>("edited_at")
        };
    }
}