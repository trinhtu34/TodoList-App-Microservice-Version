using GroupService.Application.DirectMessages.Commands;
using GroupService.Application.DirectMessages.Queries;
using GroupService.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace GroupService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DirectMessageController : ControllerBase
{
    private readonly IMediator _mediator;

    public DirectMessageController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string GetUserId()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            return jsonToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? string.Empty;
        }
        return string.Empty;
    }

    [HttpPost]
    public async Task<ActionResult<DirectMessageResponse>> CreateOrGetDirectMessage([FromBody] CreateDirectMessageRequest request)
    {
        var userId = GetUserId();
        var command = new CreateOrGetDirectMessageCommand(userId, request.OtherUserId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<DirectMessageResponse>>> GetUserDirectMessages()
    {
        var userId = GetUserId();
        var query = new GetUserDirectMessagesQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
