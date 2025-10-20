using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TagService.Application.Tags.Commands;
using TagService.Application.Tags.Queries;
using TagService.Application.TodoTags.Commands;
using TagService.Application.TodoTags.Queries;
using TagService.DTOs;

namespace TagService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TagController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagController(IMediator mediator)
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TagResponse>>> GetTags([FromQuery] int? groupId)
        {
            var cognitoSub = GetCognitoSub();
            if (string.IsNullOrEmpty(cognitoSub))
                return Ok(new List<TagResponse>()); // Return empty if no auth
            
            var query = new GetTagsQuery(cognitoSub, groupId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<TagResponse>> GetTag(int id)
        {
            var cognitoSub = GetCognitoSub();
            var query = new GetTagByIdQuery(id, cognitoSub);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "PremiumOnly")]
        public async Task<ActionResult<TagResponse>> CreateTag([FromBody] CreateTagRequest request)
        {
            var cognitoSub = GetCognitoSub();
            var command = new CreateTagCommand(request.TagName, request.Color, request.GroupId, cognitoSub);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTag), new { id = result.TagId }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "PremiumOnly")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagRequest request)
        {
            var cognitoSub = GetCognitoSub();
            var command = new UpdateTagCommand(id, request.TagName, request.Color, cognitoSub);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "PremiumOnly")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var cognitoSub = GetCognitoSub();
            var command = new DeleteTagCommand(id, cognitoSub);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("todo/{todoId}")]
        [Authorize(Policy = "PremiumOnly")]
        public async Task<IActionResult> AddTagToTodo(int todoId, [FromBody] AddTagToTodoRequest request)
        {
            var cognitoSub = GetCognitoSub();
            var command = new AddTagToTodoCommand(todoId, request.TagId, cognitoSub);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("todo/{todoId}/tag/{tagId}")]
        [Authorize(Policy = "PremiumOnly")]
        public async Task<IActionResult> RemoveTagFromTodo(int todoId, int tagId)
        {
            var cognitoSub = GetCognitoSub();
            var command = new RemoveTagFromTodoCommand(todoId, tagId, cognitoSub);
            await _mediator.Send(command);
            return NoContent();
        }


        [HttpGet("todo/{todoId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TagResponse>>> GetTagsForTodo(int todoId)
        {
            var cognitoSub = GetCognitoSub();
            var query = new GetTagsForTodoQuery(todoId, cognitoSub);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("cleanup/todo/{todoId}")]
        [Authorize]
        public async Task<IActionResult> RemoveTagsForTodo(int todoId)
        {
            var command = new RemoveTagsForTodoCommand(todoId);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
