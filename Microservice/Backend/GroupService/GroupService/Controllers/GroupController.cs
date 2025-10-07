using GroupService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace GroupService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly ILogger<GroupController> _logger;
        private readonly GroupServiceDbContext _groupServiceDbContext;
        public GroupController(GroupServiceDbContext groupServiceDbContext , ILogger<GroupController> logger)
        {
            _groupServiceDbContext = groupServiceDbContext;
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

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupsR group)
        {
            try
            {
                var cognitoSub = GetCognitoSub();
                if (string.IsNullOrEmpty(cognitoSub))
                {
                    return Unauthorized("Invalid or missing JWT token.");
                }
                group.CreatedBy = cognitoSub;
                group.CreatedAt = DateTime.UtcNow;
                _groupServiceDbContext.GroupsRs.Add(group);
                await _groupServiceDbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetGroupById), new { id = group.Id }, group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
