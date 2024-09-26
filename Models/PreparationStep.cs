using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models;

public class PreparationStep
{
    [Key]
    public int StepId { get; set; }

    [Required, ForeignKey("Recipe")]
    public int RecipeId { get; set; }  // Foreign key to Recipe


    [Required, Range(1, 50)]
    public uint StepNumber { get; set; }  // Step order number

    [Required]
    public string Description { get; set; }  // Description of the step
}