using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using TagService.DTOs;
using TagService.Models;

namespace TagService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TagController : ControllerBase
    {
        private readonly TagServiceDbContext _context;
        private readonly ILogger<TagController> _logger;

        public TagController(TagServiceDbContext context, ILogger<TagController> logger)
        {
            _context = context;
            _logger = logger;
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
        [Authorize(Policy = "PremiumOnly")]
        public async Task<ActionResult<IEnumerable<TagResponse>>> GetTags([FromQuery] int? groupId)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                if (string.IsNullOrEmpty(cognitoSub))
                    return Unauthorized();

                var query = _context.Tags.Where(t => t.CognitoSub == cognitoSub);

               
                if (groupId.HasValue)
                {
                    query = query.Where(t => t.GroupId == groupId.Value);
                }

                var tags = await query
                    .Select(t => new TagResponse
                    {
                        TagId = t.TagId,
                        TagName = t.TagName,
                        Color = t.Color,
                        GroupId = t.GroupId,
                        CreatedAt = t.CreatedAt
                    })
                    .ToListAsync();

                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tags");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "PremiumOnly")]
        public async Task<ActionResult<TagResponse>> GetTag(int id)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                if (string.IsNullOrEmpty(cognitoSub))
                    return Unauthorized();

                var tag = await _context.Tags
                    .Where(t => t.TagId == id && t.CognitoSub == cognitoSub)
                    .Select(t => new TagResponse
                    {
                        TagId = t.TagId,
                        TagName = t.TagName,
                        Color = t.Color,
                        GroupId = t.GroupId,
                        CreatedAt = t.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (tag == null)
                    return NotFound();

                return Ok(tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tag {TagId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        [Authorize(Policy = "PremiumOnly")]
        public async Task<ActionResult<TagResponse>> CreateTag([FromBody] CreateTagRequest request)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                if (string.IsNullOrEmpty(cognitoSub))
                    return Unauthorized();
                
                // nhớ bỏ mấy cái trùng 
                var exists = await _context.Tags
                    .AnyAsync(t => t.TagName == request.TagName 
                                && t.CognitoSub == cognitoSub 
                                && t.GroupId == request.GroupId);

                if (exists)
                {
                    return BadRequest(new { message = "Tag with this name already exists" });
                }

                var tag = new Tag
                {
                    TagName = request.TagName,
                    Color = request.Color ?? "#808080",
                    CognitoSub = cognitoSub,
                    GroupId = request.GroupId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();

                var response = new TagResponse
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName,
                    Color = tag.Color,
                    GroupId = tag.GroupId,
                    CreatedAt = tag.CreatedAt
                };

                return CreatedAtAction(nameof(GetTag), new { id = tag.TagId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tag");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "PremiumOnly")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagRequest request)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                if (string.IsNullOrEmpty(cognitoSub))
                    return Unauthorized();

                var tag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.TagId == id && t.CognitoSub == cognitoSub);

                if (tag == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(request.TagName))
                {
                    var exists = await _context.Tags
                        .AnyAsync(t => t.TagName == request.TagName 
                                    && t.CognitoSub == cognitoSub 
                                    && t.GroupId == tag.GroupId 
                                    && t.TagId != id);

                    if (exists)
                    {
                        return BadRequest(new { message = "Tag with this name already exists" });
                    }

                    tag.TagName = request.TagName;
                }

                if (!string.IsNullOrEmpty(request.Color))
                {
                    tag.Color = request.Color;
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tag {TagId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // delete tag và xóa luôn trong bảng liên kết
        [HttpDelete("{id}")]
        [Authorize(Policy = "PremiumOnly")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                if (string.IsNullOrEmpty(cognitoSub))
                    return Unauthorized();

                var tag = await _context.Tags
                    .Include(t => t.TodoTags)
                    .FirstOrDefaultAsync(t => t.TagId == id && t.CognitoSub == cognitoSub);

                if (tag == null)
                    return NotFound();

                if (tag.TodoTags.Any())
                {
                    _context.TodoTags.RemoveRange(tag.TodoTags);
                }

                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tag {TagId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // xác minh tag thuộc về user đó không , 
        [HttpPost("todo/{todoId}")]
        [Authorize(Policy = "PremiumOnly")]
        public async Task<IActionResult> AddTagToTodo(int todoId, [FromBody] AddTagToTodoRequest request)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                if (string.IsNullOrEmpty(cognitoSub))
                    return Unauthorized();

                var tag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.TagId == request.TagId && t.CognitoSub == cognitoSub);

                if (tag == null)
                    return NotFound(new { message = "Tag not found" });

                // kiểm tra xem tag đã được thêm vào todo chưa
                var exists = await _context.TodoTags
                    .AnyAsync(tt => tt.TodoId == todoId && tt.TagId == request.TagId);

                if (exists)
                    return BadRequest(new { message = "Tag already added to this todo" });

                var todoTag = new TodoTag
                {
                    TodoId = todoId,
                    TagId = request.TagId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.TodoTags.Add(todoTag);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tag to todo");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // xóa tag khổi 1 todo list 
        [HttpDelete("todo/{todoId}/tag/{tagId}")]
        [Authorize(Policy = "PremiumOnly")]
        public async Task<IActionResult> RemoveTagFromTodo(int todoId, int tagId)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                if (string.IsNullOrEmpty(cognitoSub))
                    return Unauthorized();

                var tag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.TagId == tagId && t.CognitoSub == cognitoSub);

                if (tag == null)
                    return NotFound(new { message = "Tag not found" });

                var todoTag = await _context.TodoTags
                    .FirstOrDefaultAsync(tt => tt.TodoId == todoId && tt.TagId == tagId);

                if (todoTag == null)
                    return NotFound(new { message = "Tag not associated with this todo" });

                _context.TodoTags.Remove(todoTag);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing tag from todo");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        [HttpGet("todo/{todoId}")]
        [Authorize(Policy = "PremiumOnly")]
        public async Task<ActionResult<IEnumerable<TagResponse>>> GetTagsForTodo(int todoId)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                if (string.IsNullOrEmpty(cognitoSub))
                    return Unauthorized();

                var tags = await _context.TodoTags
                    .Where(tt => tt.TodoId == todoId)
                    .Include(tt => tt.Tag)
                    .Where(tt => tt.Tag.CognitoSub == cognitoSub)
                    .Select(tt => new TagResponse
                    {
                        TagId = tt.Tag.TagId,
                        TagName = tt.Tag.TagName,
                        Color = tt.Tag.Color,
                        GroupId = tt.Tag.GroupId,
                        CreatedAt = tt.Tag.CreatedAt
                    })
                    .ToListAsync();

                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tags for todo {TodoId}", todoId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("test")]
        [AllowAnonymous]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var count = await _context.Tags.CountAsync();
                return Ok(new { canConnect, tagCount = count, message = "Database connection OK" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        // DELETE: api/tag/cleanup/todo/{todoId}
        // Remove all tags for a todo (called by TodoService when deleting todo)
        [HttpDelete("cleanup/todo/{todoId}")]
        [AllowAnonymous] // For inter-service communication
        public async Task<IActionResult> RemoveTagsForTodo(int todoId)
        {
            try
            {
                var todoTags = await _context.TodoTags
                    .Where(tt => tt.TodoId == todoId)
                    .ToListAsync();

                if (todoTags.Any())
                {
                    _context.TodoTags.RemoveRange(todoTags);
                    await _context.SaveChangesAsync();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing tags for todo {TodoId}", todoId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
