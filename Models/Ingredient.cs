using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models;

public class Ingredient
{
    [Key]
    public int IngredientId { get; set; }

    [Required, ForeignKey("Recipe")]
    public int RecipeId { get; set; }
    
    [Required, StringLength(255)]
    public string Name { get; set; }
    
    [Required, StringLength(100)]
    public string Quantity { get; set; }

    [Required]
    public bool IsAllergen { get; set; }
    
    public virtual Recipe Recipe { get; set; }
}