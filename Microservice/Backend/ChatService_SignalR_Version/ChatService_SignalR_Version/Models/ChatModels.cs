using Cassandra;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;

namespace ChatService_SignalR_Version.Models;


public class ChatMessage
{
    public int GroupId { get; set; }           // Changed from Guid ChatId to int GroupId
    public Guid MessageId { get; set; }        // TIMEUUID converted to Guid
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;  // Changed from Guid to string (cognito_sub)
    public string MessageText { get; set; } = string.Empty;  // Changed from Content to MessageText
    public string MessageType { get; set; } = "text";   // Changed from enum to string

    public Dictionary<string, string> Metadata { get; set; } = new();  // Added
    public bool IsEdited { get; set; } = false;         // Added
    public bool IsDeleted { get; set; } = false;        // Added
    public DateTime? EditedAt { get; set; }             // Added
}

public class SendMessageRequest
{
    [Required]
    public int GroupId { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;  // Add UserId
    
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string MessageText { get; set; } = string.Empty;
    
    public string MessageType { get; set; } = "text";
    public Dictionary<string, string> Metadata { get; set; } = new();
}

