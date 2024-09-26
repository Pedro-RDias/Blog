using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Blog.Models;

public class Comment
{
    [ForeignKey("Recipe")]
    public int RecipeId { get; set; }
    
    [ForeignKey("User")]
    public string AuthorId { get; set; }
    
    [Required, MaxLength(500)]
    public string Content { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.Now;
    public DateTime DateUpdated { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User Author { get; set; }
    
    [ForeignKey("RecipeId")]
    public virtual Recipe Recipe { get; set; }
}