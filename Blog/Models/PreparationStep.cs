using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models;

public class PreparationStep
{
    [Key] public int StepId { get; set; }
    
    [Required, Range(1, 50)] public uint StepNumber { get; set; }
    [Required] public string Description { get; set; }

    // Foreign keys
    [Required, ForeignKey("Recipe")] public int RecipeId { get; set; }
    
    // Navigation properties
    public virtual Recipe Recipe { get; set; }
}