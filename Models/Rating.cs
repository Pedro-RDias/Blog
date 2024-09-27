using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Blog.Models;


[PrimaryKey("RecipeId", "UserId")]
public class Rating
{
    [Required, ForeignKey("Recipe")]
    public int RecipeId { get; set; }

    [Required, ForeignKey("User")]
    public string UserId { get; set; }
    
    [Required, Range(1, 5)]
    public int RatingValue { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.Now;
    public DateTime DateUpdated { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual User User { get; set; }
    public virtual Recipe Recipe { get; set; }
}
