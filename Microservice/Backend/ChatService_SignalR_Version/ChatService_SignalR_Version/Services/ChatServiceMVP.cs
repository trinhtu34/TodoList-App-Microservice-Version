using ChatService_SignalR_Version.Models;
using ChatService_SignalR_Version.Repositories;
using Microsoft.AspNetCore.SignalR;
using ChatService_SignalR_Version.Hub;

namespace ChatService_SignalR_Version.Services;

public interface IChatService
{
    // Messages
    Task<ChatMessage> SaveMessageAsync(ChatMessage message);
    Task<List<ChatMessage>> GetMessagesAsync(Guid chatId, int limit = 50, Guid? beforeMessageId = null);
    Task<ChatMessage?> GetMessageAsync(Guid chatId, Guid messageId);

}
public class ChatServiceMVP : IChatService
{
    private readonly IMessageRepository _messageRepo;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<ChatServiceMVP> _logger;

    public ChatServiceMVP(
        IMessageRepository messageRepo,
        IHubContext<ChatHub> hubContext,
        ILogger<ChatServiceMVP> logger)
    {
        _messageRepo = messageRepo;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<ChatMessage> SendMessageAsync(SendMessageRequest request)
    {
        // Create message
        var message = new ChatMessage
        {
            GroupId = request.GroupId,
            UserId = request.UserId,
            MessageText = request.MessageText,
            MessageType = request.MessageType,
            Metadata = request.Metadata,
            IsEdited = false,
            IsDeleted = false
        };

        // Save to ScyllaDB
        var savedMessage = await _messageRepo.SaveAsync(message);

        // Broadcast to SignalR group
        await _hubContext.Clients.Group($"group_{request.GroupId}")
            .SendAsync("ReceiveMessage", savedMessage);

        _logger.LogInformation($"Message sent and broadcasted: {savedMessage.MessageId}");
        return savedMessage;
    }

    public async Task<List<ChatMessage>> GetMessagesAsync(int groupId, int limit = 50)
    {
        return await _messageRepo.GetByGroupAsync(groupId, limit);
    }

    public async Task<ChatMessage?> GetMessageAsync(int groupId, Guid messageId)
    {
        return await _messageRepo.GetByIdAsync(groupId, messageId);
    }
}