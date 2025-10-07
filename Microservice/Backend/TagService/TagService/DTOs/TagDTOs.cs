using System.ComponentModel.DataAnnotations;

namespace TagService.DTOs
{
    // Response DTO
    public class TagResponse
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
        public string Color { get; set; }
        public int? GroupId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    // Create Tag Request
    public class CreateTagRequest
    {
        [Required]
        [MaxLength(50)]
        public string TagName { get; set; }

        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be valid hex format (e.g., #FF5733)")]
        public string Color { get; set; } = "#808080";

        public int? GroupId { get; set; }
    }

    // Update Tag Request
    public class UpdateTagRequest
    {
        [MaxLength(50)]
        public string TagName { get; set; }

        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be valid hex format (e.g., #FF5733)")]
        public string Color { get; set; }
    }

    // Add Tag to Todo Request
    public class AddTagToTodoRequest
    {
        [Required]
        public int TagId { get; set; }
    }
}
