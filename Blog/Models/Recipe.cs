using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blog.Data.Enums;

namespace Blog.Models;

public class Recipe
{
    [Key] public int Id { get; set; }

    [Required, StringLength(255)] public string Title { get; set; }
    [Required, StringLength(500)] public string Summary { get; set; }
    [Required] public Category Category { get; set; }
    [Required] public Diet Diet { get; set; }

    // Foreign keys
    [Required, ForeignKey("User")] public string AuthorId { get; set; }

    // Timestamps
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public DateTime DateUpdated { get; set; } = DateTime.Now;
    
    // Navigation properties
    [ForeignKey("AuthorId")] public virtual User Author { get; set; }
    [ForeignKey("RecipeId")] public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    [ForeignKey("RecipeId")]
    public virtual ICollection<PreparationStep> PreparationSteps { get; set; } = new List<PreparationStep>();

    [ForeignKey("RecipeId")] public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    [ForeignKey("RecipeId")] public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}