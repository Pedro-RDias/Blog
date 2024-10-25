using Blog.Models;

namespace Blog.ViewModels;

public class RecipesViewIndexModel
{
    public List<Recipe> Recipes { get; set; }
    public Dictionary<int, List<int>> Dates { get; set; }
}