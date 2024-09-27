using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models;

public class Ingredient
{
    [Key] public int IngredientId { get; set; }
    [Required, StringLength(255)] public string Name { get; set; }
    [Required, StringLength(100)] public string Quantity { get; set; }
    [Required] public bool IsAllergen { get; set; }
    
    // Foreign keys
    [Required, ForeignKey("Recipe")] public int RecipeId { get; set; }

    // Navigation properties
    public virtual Recipe Recipe { get; set; }
}