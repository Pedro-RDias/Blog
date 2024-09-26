using System.ComponentModel.DataAnnotations;

namespace Blog.Models;

public class PreparationStep
{
    [Key]
    public int StepId { get; set; }

    [Required]
    public int RecipeId { get; set; }  // Foreign key to Recipe

    [Required]
    public int StepNumber { get; set; }  // Step order number

    [Required]
    public string Description { get; set; }  // Description of the step

    // Navigation property
    public virtual Recipe Recipe { get; set; }
}