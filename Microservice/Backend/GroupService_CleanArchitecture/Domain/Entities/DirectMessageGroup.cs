using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Quick lookup for existing 1-1 conversations between two users
/// </summary>
public class DirectMessageGroup : BaseEntity
{
    /// <summary>
    /// Smaller cognito_sub (alphabetically)
    /// </summary>
    public string User1Id { get; set; } = null!;

    /// <summary>
    /// Larger cognito_sub (alphabetically)
    /// </summary>
    public string User2Id { get; set; } = null!;

    public int GroupId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Group Group { get; set; } = null!;
}
