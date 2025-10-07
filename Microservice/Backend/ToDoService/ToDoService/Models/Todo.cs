using System;
using System.Collections.Generic;

namespace ToDoService.Models;

public partial class Todo
{
    public int TodoId { get; set; }

    public string Description { get; set; } = null!;

    public bool? IsDone { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    /// <summary>
    /// Owner/Creator
    /// </summary>
    public string CognitoSub { get; set; } = null!;

    /// <summary>
    /// NULL = personal todo, NOT NULL = group todo
    /// </summary>
    public int? GroupId { get; set; }

    /// <summary>
    /// NULL = unassigned, cognito_sub of assignee
    /// </summary>
    public string? AssignedTo { get; set; }
}
