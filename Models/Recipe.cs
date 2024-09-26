using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models;

public class Recipe
{
    [Key]
    public int RecipeId { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; }

    [StringLength(500)]
    public string Summary { get; set; }  // Short summary of the recipe

    [Required]
    [StringLength(100)]
    public string Category { get; set; }  // Category like "Dessert", "Main Course"

    [Required]
    public string AuthorId { get; set; }  // Foreign key from AspNetUsers

    public DateTime DateCreated { get; set; } = DateTime.Now;

    public DateTime? DateUpdated { get; set; }

    [Required]
    public bool IsPublic { get; set; } = false;

    // Navigation properties
    // [ForeignKey("AuthorId")]
    // public virtual ApplicationUser Author { get; set; }  // The user who created the recipe
    
    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    
    public virtual ICollection<PreparationStep> PreparationSteps { get; set; } = new List<PreparationStep>();
    
    public virtual ICollection<AllergicIngredient> AllergicIngredients { get; set; } = new List<AllergicIngredient>();
    
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();  // Comments on the recipe
    
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();  // Ratings for the recipe

}