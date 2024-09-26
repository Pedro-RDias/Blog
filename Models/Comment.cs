using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models;

public class Comment
{
    [Key]
    public int CommentId { get; set; }

    [Required]
    public int RecipeId { get; set; }  // Foreign key to Recipe

    [Required]
    public string AuthorId { get; set; }  // Foreign key to AspNetUsers

    [Required]
    public string Content { get; set; }  // Comment text

    public DateTime DateCreated { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("AuthorId")]
    public virtual User Author { get; set; }  // The user who created the comment

    public virtual Recipe Recipe { get; set; }
}