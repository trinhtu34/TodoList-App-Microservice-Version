using Application.DTOs;
using Application.Features.Invitations.Commands;
using Application.Features.Invitations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InvitationController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvitationController(IMediator mediator)
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
    public async Task<ActionResult<InvitationResponse>> CreateInvitation([FromBody] CreateInvitationRequest request)
    {
        var userId = GetUserId();
        var command = new CreateInvitationCommand(request.GroupId, request.InvitedUser, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<InvitationResponse>>> GetUserInvitations()
    {
        var userId = GetUserId();
        var query = new GetUserInvitationsQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("{invitationId}/accept")]
    public async Task<ActionResult> AcceptInvitation(int invitationId)
    {
        var userId = GetUserId();
        var command = new AcceptInvitationCommand(invitationId, userId);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{invitationId}/decline")]
    public async Task<ActionResult> DeclineInvitation(int invitationId)
    {
        var userId = GetUserId();
        var command = new DeclineInvitationCommand(invitationId, userId);
        await _mediator.Send(command);
        return NoContent();
    }
}
