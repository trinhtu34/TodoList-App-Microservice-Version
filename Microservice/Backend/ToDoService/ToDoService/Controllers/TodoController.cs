using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using ToDoService.DTOs;
using ToDoService.Models;
using ToDoService.ServiceClients;

namespace ToDoService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodosController : ControllerBase
    {
        private readonly TodoServiceDbContext _context;
        private readonly ILogger<TodosController> _logger;
        private readonly ITagServiceClient _tagServiceClient;
        private readonly IGroupServiceClient _groupServiceClient;

        public TodosController(
            TodoServiceDbContext context,
            ILogger<TodosController> logger,
            ITagServiceClient tagServiceClient,
            IGroupServiceClient groupServiceClient)
        {
            _context = context;
            _logger = logger;
            _tagServiceClient = tagServiceClient;
            _groupServiceClient = groupServiceClient;
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

        // GET: api/todos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoResponse>>> GetTodos([FromQuery] int? groupId)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                if (string.IsNullOrEmpty(cognitoSub))
                    return Unauthorized();

                var query = _context.Todos.AsQueryable();

                if (groupId.HasValue)
                {
                    // Verify membership
                    var isMember = await _groupServiceClient.VerifyMembership(groupId.Value, cognitoSub);
                    if (!isMember)
                        return Forbid();

                    query = query.Where(t => t.GroupId == groupId.Value);
                }
                else
                {
                    // Personal todos
                    query = query.Where(t => t.CognitoSub == cognitoSub && t.GroupId == null);
                }

                var todos = await query.ToListAsync();
                var token = GetToken();

                var responses = new List<TodoResponse>();
                foreach (var todo in todos)
                {
                    var tags = await _tagServiceClient.GetTagsForTodo(todo.TodoId, token);
                    responses.Add(new TodoResponse
                    {
                        TodoId = todo.TodoId,
                        Description = todo.Description,
                        IsDone = todo.IsDone,
                        DueDate = todo.DueDate,
                        CreateAt = todo.CreateAt,
                        UpdateAt = todo.UpdateAt,
                        GroupId = todo.GroupId,
                        AssignedTo = todo.AssignedTo,
                        Tags = tags.Select(t => new TagResponse
                        {
                            TagId = t.TagId,
                            TagName = t.TagName,
                            Color = t.Color
                        }).ToList()
                    });
                }

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todos");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // GET: api/todos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoResponse>> GetTodo(int id)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                var todo = await _context.Todos.FindAsync(id);

                if (todo == null)
                    return NotFound();

                // Verify access
                if (todo.GroupId.HasValue)
                {
                    var isMember = await _groupServiceClient.VerifyMembership(todo.GroupId.Value, cognitoSub);
                    if (!isMember)
                        return Forbid();
                }
                else if (todo.CognitoSub != cognitoSub)
                {
                    return Forbid();
                }

                var token = GetToken();
                var tags = await _tagServiceClient.GetTagsForTodo(todo.TodoId, token);

                return Ok(new TodoResponse
                {
                    TodoId = todo.TodoId,
                    Description = todo.Description,
                    IsDone = todo.IsDone,
                    DueDate = todo.DueDate,
                    CreateAt = todo.CreateAt,
                    UpdateAt = todo.UpdateAt,
                    GroupId = todo.GroupId,
                    AssignedTo = todo.AssignedTo,
                    Tags = tags.Select(t => new TagResponse
                    {
                        TagId = t.TagId,
                        TagName = t.TagName,
                        Color = t.Color
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todo {TodoId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // POST: api/todos
        [HttpPost]
        public async Task<ActionResult<TodoResponse>> CreateTodo([FromBody] CreateTodoRequest request)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                if (string.IsNullOrEmpty(cognitoSub))
                    return Unauthorized();

                // Verify group membership if group todo
                if (request.GroupId.HasValue)
                {
                    var isMember = await _groupServiceClient.VerifyMembership(request.GroupId.Value, cognitoSub);
                    if (!isMember)
                        return Forbid();
                }

                var todo = new Todo
                {
                    Description = request.Description,
                    DueDate = request.DueDate,
                    CognitoSub = cognitoSub,
                    GroupId = request.GroupId,
                    AssignedTo = request.AssignedTo,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow,
                    IsDone = false
                };

                _context.Todos.Add(todo);
                await _context.SaveChangesAsync();

                // Add tags
                var token = GetToken();
                foreach (var tagId in request.TagIds)
                {
                    await _tagServiceClient.AddTagToTodo(todo.TodoId, tagId, token);
                }

                var tags = await _tagServiceClient.GetTagsForTodo(todo.TodoId, token);

                return CreatedAtAction(nameof(GetTodo), new { id = todo.TodoId }, new TodoResponse
                {
                    TodoId = todo.TodoId,
                    Description = todo.Description,
                    IsDone = todo.IsDone,
                    DueDate = todo.DueDate,
                    CreateAt = todo.CreateAt,
                    UpdateAt = todo.UpdateAt,
                    GroupId = todo.GroupId,
                    AssignedTo = todo.AssignedTo,
                    Tags = tags.Select(t => new TagResponse
                    {
                        TagId = t.TagId,
                        TagName = t.TagName,
                        Color = t.Color
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // PUT: api/todos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoRequest request)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                var todo = await _context.Todos.FindAsync(id);

                if (todo == null)
                    return NotFound();

                // Verify access
                if (todo.GroupId.HasValue)
                {
                    var isMember = await _groupServiceClient.VerifyMembership(todo.GroupId.Value, cognitoSub);
                    if (!isMember)
                        return Forbid();
                }
                else if (todo.CognitoSub != cognitoSub)
                {
                    return Forbid();
                }

                // Update fields
                if (!string.IsNullOrEmpty(request.Description))
                    todo.Description = request.Description;
                if (request.IsDone.HasValue)
                    todo.IsDone = request.IsDone.Value;
                if (request.DueDate.HasValue)
                    todo.DueDate = request.DueDate.Value;
                if (request.AssignedTo != null)
                    todo.AssignedTo = request.AssignedTo;

                todo.UpdateAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo {TodoId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // DELETE: api/todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                var todo = await _context.Todos.FindAsync(id);

                if (todo == null)
                    return NotFound();

                // Verify access
                if (todo.GroupId.HasValue)
                {
                    var isMember = await _groupServiceClient.VerifyMembership(todo.GroupId.Value, cognitoSub);
                    if (!isMember)
                        return Forbid();
                }
                else if (todo.CognitoSub != cognitoSub)
                {
                    return Forbid();
                }

                // Cleanup tags
                await _tagServiceClient.RemoveTagsForTodo(id);

                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo {TodoId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // DELETE: api/todos/{todoId}/tag/{tagId}
        [HttpDelete("{todoId}/tag/{tagId}")]
        public async Task<IActionResult> RemoveTagFromTodo(int todoId, int tagId)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                var todo = await _context.Todos.FindAsync(todoId);

                if (todo == null)
                    return NotFound();

                // Verify access
                if (todo.GroupId.HasValue)
                {
                    var isMember = await _groupServiceClient.VerifyMembership(todo.GroupId.Value, cognitoSub);
                    if (!isMember)
                        return Forbid();
                }
                else if (todo.CognitoSub != cognitoSub)
                {
                    return Forbid();
                }

                var token = GetToken();
                await _tagServiceClient.RemoveTagFromTodo(todoId, tagId, token);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing tag from todo");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // GET: api/todos/verify/{todoId}/owner/{userId}
        [HttpGet("verify/{todoId}/owner/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> VerifyTodoOwnership(int todoId, string userId)
        {
            try
            {
                var todo = await _context.Todos.FindAsync(todoId);
                if (todo == null)
                    return Ok(false);

                if (todo.GroupId.HasValue)
                {
                    var isMember = await _groupServiceClient.VerifyMembership(todo.GroupId.Value, userId);
                    return Ok(isMember);
                }

                return Ok(todo.CognitoSub == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying todo ownership");
                return Ok(false);
            }
        }
    }
}
