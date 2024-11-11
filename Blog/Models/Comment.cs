using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Blog.Models;

public class Comment
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(500)] public string Content { get; set; }
    
    // Foreign keys
    [Required, ForeignKey("Recipe")] public int RecipeId { get; set; }
    [Required, ForeignKey("User")] public string AuthorId { get; set; }
    [ForeignKey("Comment")] public int? ReplyToId { get; set; }  // Make nullable with default value

    // Timestamps
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public DateTime DateUpdated { get; set; } = DateTime.Now;

    // Navigation properties
    [JsonIgnore]
    public virtual User? Author { get; set; }

    [JsonIgnore]
    public virtual Recipe? Recipe { get; set; }

    [JsonIgnore]
    public virtual ICollection<Comment>? Replies { get; set; }

    // Add these properties for JSON serialization
    public string? AuthorName => Author?.UserName;
}