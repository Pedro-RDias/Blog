using System.ComponentModel.DataAnnotations;

namespace Blog.Models;

public class AllergicIngredient
{
    [Key]
    public int AllergicIngredientId { get; set; }

    [Required]
    public int RecipeId { get; set; }  // Foreign key to Recipe

    [Required]
    [StringLength(255)]
    public string Ingredient { get; set; }  // Allergen name (e.g., "Peanuts", "Milk")

    // Navigation property
    public virtual Recipe Recipe { get; set; }
}