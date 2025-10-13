using GroupService.Application.Groups.Commands;
using GroupService.Application.Groups.Queries;
using GroupService.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace GroupService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GroupController : ControllerBase
{
    private readonly IMediator _mediator;

    public GroupController(IMediator mediator)
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
    public async Task<ActionResult<GroupResponse>> CreateGroup([FromBody] CreateGroupRequest request)
    {
        var userId = GetUserId();
        var command = new CreateGroupCommand(request.GroupName, request.GroupAvatar, request.GroupDescription, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<GroupListResponse>>> GetUserGroups()
    {
        var userId = GetUserId();
        var query = new GetUserGroupsQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{groupId}")]
    public async Task<ActionResult<GroupResponse>> GetGroupById(int groupId)
    {
        var userId = GetUserId();
        var query = new GetGroupByIdQuery(groupId, userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("{groupId}")]
    public async Task<ActionResult<GroupResponse>> UpdateGroup(int groupId, [FromBody] UpdateGroupRequest request)
    {
        var userId = GetUserId();
        var command = new UpdateGroupCommand(groupId, request.GroupName, request.GroupAvatar, request.GroupDescription, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{groupId}")]
    public async Task<ActionResult> DeleteGroup(int groupId)
    {
        var userId = GetUserId();
        var command = new DeleteGroupCommand(groupId, userId);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("{groupId}/archive")]
    public async Task<ActionResult> ArchiveGroup(int groupId)
    {
        var userId = GetUserId();
        var command = new ArchiveGroupCommand(groupId, userId);
        await _mediator.Send(command);
        return NoContent();
    }
}
