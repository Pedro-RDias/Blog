using System.ComponentModel.DataAnnotations;

namespace Blog.Data.Enums
{
    public enum Category
    {
        [Display(Name = "Breakfast")]
        Breakfast,

        [Display(Name = "Lunch")]
        Lunch,

        [Display(Name = "Dinner")]
        Dinner,

        [Display(Name = "Dessert")]
        Dessert,

        [Display(Name = "Snack")]
        Snack,

        [Display(Name = "Drink")]
        Drink
    }
}