using System.ComponentModel.DataAnnotations;

namespace Blog.Models;

public class Photo
{
    [Key] public int Id { get; set; }

    [Required] public string ImagePath { get; set; }

    [Required] public int RecipeId { get; set; }

    public virtual Recipe? Recipe { get; set; }

    public DateTime DateUploaded { get; set; } = DateTime.Now;
}