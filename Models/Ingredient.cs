using System.ComponentModel.DataAnnotations;

namespace Blog.Models;

public class Ingredient
{
    [Key]
    public int IngredientId { get; set; }

    [Required]
    public int RecipeId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    public string Quantity { get; set; }

    // Navigation property
    public virtual Recipe Recipe { get; set; }
}