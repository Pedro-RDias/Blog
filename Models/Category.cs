using System.ComponentModel.DataAnnotations;

namespace Blog.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }
    
    [Required]
    [MaxLength(75)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Description { get; set; }
    
    [Required]
    public string ImageUrl { get; set; }
    
    // Navigation property
    public virtual ICollection<Recipe> Recipes { get; set; }
}