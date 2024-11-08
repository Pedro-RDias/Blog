using Blog.Data.Enums;

namespace Blog.ViewModels;

public class RecipeViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Summary { get; set; }
    public Category Category { get; set; }
    public Diet Diet { get; set; }
    public List<IngredientViewModel> Ingredients { get; set; } = new();
    public List<PreparationStepViewModel> PreparationSteps { get; set; } = new();

    public List<PhotoViewModel> Photos { get; set; } = new();
    public List<IFormFile> NewPhotos { get; set; } = new();

    public override string ToString()
    {
        return "Title: " + Title + ", Summary: " + Summary + ", Category: " + Category + ", Diet: " + Diet +
               ", Ingredients: " + string.Join(", ", Ingredients) + ", Preparation steps: " +
               string.Join(", ", PreparationSteps);
    }
}

public class PhotoViewModel
{
    public int Id { get; set; }
    public string? Caption { get; set; }
    public IFormFile? ImageFile { get; set; }
    public string? ImagePath { get; set; }
}

public class IngredientViewModel
{
    public int? IngredientId { get; set; }
    public string Name { get; set; }
    public string Quantity { get; set; }
    public bool IsAllergen { get; set; }

    public override string ToString()
    {
        return "Ingredient: " + Name + ", Quantity: " + Quantity + ", IsAllergen: " + IsAllergen;
    }
}

public class PreparationStepViewModel
{
    public int? StepId { get; set; }
    public uint StepNumber { get; set; }
    public string Description { get; set; }

    public override string ToString()
    {
        return "Preparation step: " + StepNumber + ", Description: " + Description;
    }
}