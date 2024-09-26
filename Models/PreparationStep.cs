using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models;

public class PreparationStep
{
    [Key] public int StepId { get; set; }

    [Required, ForeignKey("Recipe")] public int RecipeId { get; set; }


    [Required, Range(1, 50)] public uint StepNumber { get; set; }

    [Required] public string Description { get; set; }

    [ForeignKey("RecipeId")] public virtual Recipe Recipe { get; set; }
}