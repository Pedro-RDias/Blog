using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Blog.Models;

public class Rating
{
    [Key, ForeignKey("Recipe")]
    public int RecipeId { get; set; }  // Foreign key to Recipe

    [Key, ForeignKey("User")]
    public string UserId { get; set; }  // Foreign key to AspNetUsers

    [Required]
    [Range(1, 5)]
    public int RatingValue { get; set; }  // Rating value between 1 and 5

    public DateTime DateCreated { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("AuthorId")]
    public virtual User Author { get; set; }  // The user who gave the rating

    public virtual Recipe Recipe { get; set; }

    // Ensure a user can rate a recipe only once
}