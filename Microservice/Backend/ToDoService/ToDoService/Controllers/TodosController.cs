using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using ToDoService.Application.Todos.Commands;
using ToDoService.Application.Todos.Queries;
using ToDoService.DTOs;

namespace ToDoService.Controllers;

[Route("api/todos")]
[ApiController]
[Authorize]
public class TodosController : ControllerBase
{
    private readonly IMediator _mediator;

    public TodosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string GetCognitoSub()
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

    private string GetToken()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        return string.Empty;
    }

    [HttpGet]
    public async Task<ActionResult<List<TodoResponse>>> GetTodos([FromQuery] int? groupId)
    {
        var userId = GetCognitoSub();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var query = new GetTodosQuery(userId, groupId, GetToken());
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoResponse>> GetTodo(int id)
    {
        var userId = GetCognitoSub();
        var query = new GetTodoByIdQuery(id, userId, GetToken());
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TodoResponse>> CreateTodo([FromBody] CreateTodoRequest request)
    {
        var userId = GetCognitoSub();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new CreateTodoCommand(
            request.Description,
            request.DueDate,
            request.GroupId,
            request.AssignedTo,
            request.TagIds,
            userId,
            GetToken()
        );

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTodo), new { id = result.TodoId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoRequest request)
    {
        var userId = GetCognitoSub();
        var command = new UpdateTodoCommand(
            id,
            request.Description,
            request.IsDone,
            request.DueDate,
            request.AssignedTo,
            userId
        );

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var userId = GetCognitoSub();
        var command = new DeleteTodoCommand(id, userId);
        await _mediator.Send(command);
        return NoContent();
    }
}
