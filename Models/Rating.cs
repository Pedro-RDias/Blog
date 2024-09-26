using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Blog.Models;

public class Rating
{
    [ForeignKey("Recipe")]
    public int RecipeId { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; }
    
    [Required, Range(1, 5)]
    public int RatingValue { get; set; }

    [Required]
    public DateTime DateCreated { get; set; } = DateTime.Now;
    
    [Required]
    public DateTime DateUpdated { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual User User { get; set; }
    public virtual Recipe Recipe { get; set; }
}
