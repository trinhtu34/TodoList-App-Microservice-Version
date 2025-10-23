using Microsoft.AspNetCore.SignalR;
using ChatService_SignalR_Version.Models;
using ChatService_SignalR_Version.Services;

namespace ChatService_SignalR_Version.Hub;

public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly ChatServiceMVP _chatService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ChatServiceMVP chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    // Join a group (REQUIRED for chat to work!)
    public async Task JoinGroup(int groupId)
    {
        var groupName = $"group_{groupId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        
        var userId = GetUserId();
        var userName = GetUserName();
        
        _logger.LogInformation($"User {userName} ({userId}) joined group {groupId}");

        //identify the user belong to group , user internal api call to GroupService can be added later - IMPORTANT !!!!

        // Notify others in group
        await Clients.GroupExcept(groupName, Context.ConnectionId)
            .SendAsync("UserJoined", new { userId, userName, groupId });
    }

    // Leave a group
    public async Task LeaveGroup(int groupId)
    {
        var groupName = $"group_{groupId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        
        var userId = GetUserId();
        var userName = GetUserName();
        
        _logger.LogInformation($"User {userName} ({userId}) left group {groupId}");
        
        // Notify others in group
        await Clients.Group(groupName)
            .SendAsync("UserLeft", new { userId, userName, groupId });
    }

    // Send message via SignalR (Real-time)
    public async Task SendMessageRealtime(int groupId, string messageText, string messageType = "text")
    {
        try
        {
            var userId = GetUserId();
            var userName = GetUserName();

            // Create message request
            var request = new SendMessageRequest
            {
                GroupId = groupId,
                UserId = userId,
                MessageText = messageText,
                MessageType = messageType
            };

            // Save to database via ChatServiceMVP
            var savedMessage = await _chatService.SendMessageAsync(request);

            // ChatServiceMVP already broadcasts to group, so we just confirm to sender
            await Clients.Caller.SendAsync("MessageSent", new { 
                success = true, 
                messageId = savedMessage.MessageId 
            });

            _logger.LogInformation($"Message sent to group {groupId} by user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending message to group {groupId}");
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    // Simple typing indicators (MVP: No database storage)
    public async Task StartTyping(int groupId)
    {
        var userId = GetUserId();
        var userName = GetUserName();
        var groupName = $"group_{groupId}";
        
        await Clients.GroupExcept(groupName, Context.ConnectionId)
            .SendAsync("UserStartedTyping", new { userId, userName, groupId });
    }

    public async Task StopTyping(int groupId)
    {
        var userId = GetUserId();
        var groupName = $"group_{groupId}";
        
        await Clients.GroupExcept(groupName, Context.ConnectionId)
            .SendAsync("UserStoppedTyping", new { userId, groupId });
    }

    // Connection events (MVP: Simple logging)
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        var userName = GetUserName();
        
        _logger.LogInformation($"User {userName} ({userId}) connected with connection {Context.ConnectionId}");
        
        // TODO: Add to online users in Redis (Phase 2)
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        var userName = GetUserName();
        
        _logger.LogInformation($"User {userName} ({userId}) disconnected");
        
        // TODO: Remove from online users in Redis (Phase 2)
        
        await base.OnDisconnectedAsync(exception);
    }

    // Helper methods
    private string GetUserId()
    {
        // Try to get user ID from JWT claims or use connection ID as fallback
        return Context.User?.FindFirst("sub")?.Value 
               ?? Context.User?.FindFirst("user_id")?.Value
               ?? Context.UserIdentifier
               ?? Context.ConnectionId;
    }

    private string GetUserName()
    {
        return Context.User?.FindFirst("name")?.Value
               ?? Context.User?.FindFirst("username")?.Value
               ?? Context.User?.Identity?.Name
               ?? "Anonymous";
    }
}
