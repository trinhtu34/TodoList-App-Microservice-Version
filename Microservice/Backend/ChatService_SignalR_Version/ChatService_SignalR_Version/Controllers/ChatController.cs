using Microsoft.AspNetCore.Mvc;
using ChatService_SignalR_Version.Models;
using ChatService_SignalR_Version.Services;

namespace ChatService_SignalR_Version.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatServiceMVP _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(ChatServiceMVP chatService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            var message = await _chatService.SendMessageAsync(request);
            return Ok(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            return StatusCode(500, "Error sending message");
        }
    }

    [HttpGet("messages/{groupId}")]
    public async Task<IActionResult> GetMessages(int groupId, [FromQuery] int limit = 50)
    {
        try
        {
            var messages = await _chatService.GetMessagesAsync(groupId, limit);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting messages for group {GroupId}", groupId);
            return StatusCode(500, "Error getting messages");
        }
    }

    [HttpGet("message/{groupId}/{messageId}")]
    public async Task<IActionResult> GetMessage(int groupId, Guid messageId)
    {
        try
        {
            var message = await _chatService.GetMessageAsync(groupId, messageId);
            if (message == null)
                return NotFound();
                
            return Ok(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting message {MessageId}", messageId);
            return StatusCode(500, "Error getting message");
        }
    }
}