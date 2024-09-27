using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace Blog.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }
    
    [Required, ForeignKey("Recipe")]
    public int RecipeId { get; set; }
    
    [Required, ForeignKey("User")]
    public string AuthorId { get; set; }
    
    [Required, MaxLength(500)]
    public string Content { get; set; }


    [ForeignKey("Comment")]
    public int ReplyToId { get; set; }
    
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public DateTime DateUpdated { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual User Author { get; set; }
    public virtual Recipe Recipe { get; set; }
}