using System.ComponentModel.DataAnnotations;

namespace Blog.Data.Enums;

public enum Diet
{
    [Display(Name = "Vegan")]
    Vegan,
    
    [Display(Name = "Vegetarian")]
    Vegetarian,
    
    [Display(Name = "Pescatarian")]
    Pescatarian,
    
    [Display(Name = "Omnivore")]
    Omnivore
}