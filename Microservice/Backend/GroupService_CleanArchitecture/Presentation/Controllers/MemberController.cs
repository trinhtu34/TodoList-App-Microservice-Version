using Application.DTOs;
using Application.Features.Members.Commands;
using Application.Features.Members.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Presentation.Controllers;

[Route("api/groups/{groupId}/[controller]")]
[ApiController]
[Authorize]
public class MemberController : ControllerBase
{
    private readonly IMediator _mediator;

    public MemberController(IMediator mediator)
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

    [HttpGet]
    public async Task<ActionResult<List<MemberResponse>>> GetMembers(int groupId)
    {
        var userId = GetUserId();
        var query = new GetGroupMembersQuery(groupId, userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpDelete("{memberUserId}")]
    public async Task<ActionResult> RemoveMember(int groupId, string memberUserId)
    {
        var userId = GetUserId();
        var command = new RemoveMemberCommand(groupId, memberUserId, userId);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("leave")]
    public async Task<ActionResult> LeaveGroup(int groupId)
    {
        var userId = GetUserId();
        var command = new LeaveGroupCommand(groupId, userId);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("{memberUserId}/role")]
    public async Task<ActionResult> UpdateMemberRole(int groupId, string memberUserId, [FromBody] UpdateMemberRoleRequest request)
    {
        var userId = GetUserId();
        var command = new UpdateMemberRoleCommand(groupId, memberUserId, request.Role, userId);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("settings")]
    public async Task<ActionResult> UpdateSettings(int groupId, [FromBody] UpdateMemberNicknameRequest request)
    {
        var userId = GetUserId();
        var command = new UpdateMemberSettingsCommand(groupId, request.Nickname, null, userId);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("mute")]
    public async Task<ActionResult> ToggleMute(int groupId, [FromQuery] bool muted)
    {
        var userId = GetUserId();
        var command = new UpdateMemberSettingsCommand(groupId, null, muted, userId);
        await _mediator.Send(command);
        return NoContent();
    }
}
