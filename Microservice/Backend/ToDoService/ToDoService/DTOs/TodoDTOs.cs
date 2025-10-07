using System.ComponentModel.DataAnnotations;

namespace ToDoService.DTOs
{
    public class TodoResponse
    {
        public int TodoId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool? IsDone { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? GroupId { get; set; }
        public string? AssignedTo { get; set; }
        public List<TagResponse> Tags { get; set; } = new List<TagResponse>();
    }

    public class CreateTodoRequest
    {
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        public int? GroupId { get; set; }

        public string? AssignedTo { get; set; }

        public List<int> TagIds { get; set; } = new List<int>();
    }

    public class UpdateTodoRequest
    {
        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool? IsDone { get; set; }

        public DateTime? DueDate { get; set; }

        public string? AssignedTo { get; set; }

        public List<int>? TagIds { get; set; }
    }

    public class TagResponse
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
        public string? Color { get; set; }
    }
}