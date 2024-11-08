using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Controllers;

public class RecipesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;


    public RecipesController(
        ApplicationDbContext context,
        UserManager<User> userManager
    )
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Recipes
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Recipes.Include(r => r.Author);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: Recipes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var recipe = await _context.Recipes
            .Include(r => r.Author)
            .Include(r => r.Comments)
            .Include(r => r.Ratings)
            .Include(r => r.Ingredients)
            .Include(r => r.PreparationSteps)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (recipe == null) return NotFound();

        return View(recipe);
    }

    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult Create()
    {
        return View(new RecipeViewModel());
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RecipeViewModel viewModel, string IngredientsJson, string StepsJson,
        List<IFormFile> photoFiles)
    {
        var ingredients = JsonConvert.DeserializeObject<List<IngredientViewModel>>(IngredientsJson);
        var steps = JsonConvert.DeserializeObject<List<PreparationStepViewModel>>(StepsJson);
        if (ModelState.IsValid)
        {
            var recipe = new Recipe
            {
                Title = viewModel.Title,
                Summary = viewModel.Summary,
                Category = viewModel.Category,
                Diet = viewModel.Diet,
                Published = viewModel.Published,
                AuthorId = _userManager.GetUserId(User),
                Ingredients = ingredients.Select(i => new Ingredient
                {
                    Name = i.Name,
                    Quantity = i.Quantity,
                    IsAllergen = i.IsAllergen
                }).ToList(),
                PreparationSteps = steps.Select((s, index) => new PreparationStep
                {
                    StepNumber = (uint)index + 1,
                    Description = s.Description
                }).ToList()
            };

            // Handle photo uploads
            if (photoFiles != null && photoFiles.Any())
            {
                recipe.Photos = new List<Photo>();
                foreach (var file in photoFiles)
                    if (file.Length > 0)
                    {
                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                        var filePath = Path.Combine("wwwroot", "uploads", "recipes", fileName);

                        // Ensure directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        // Save file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Create photo record
                        recipe.Photos.Add(new Photo
                        {
                            ImagePath = $"/uploads/recipes/{fileName}"
                        });
                    }
            }

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        viewModel.Ingredients = ingredients;
        viewModel.PreparationSteps = steps;

        return View(viewModel);
    }

    // GET: Recipes/Edit/5
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.PreparationSteps)
            .Include(r => r.Photos)  // Include photos
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return NotFound();

        var recipeVm = new RecipeViewModel
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Summary = recipe.Summary,
            Category = recipe.Category,
            Diet = recipe.Diet,
            Published = recipe.Published,
            Photos = recipe.Photos.Select(p => new PhotoViewModel
            {
                Id = p.Id,
                ImagePath = p.ImagePath
            }).ToList()
        };

        foreach (var ingredient in recipe.Ingredients)
            recipeVm.Ingredients.Add(new IngredientViewModel
            {
                IngredientId = ingredient.IngredientId,
                Name = ingredient.Name,
                Quantity = ingredient.Quantity,
                IsAllergen = ingredient.IsAllergen
            });

        var StepNumb = 1;
        foreach (var step in recipe.PreparationSteps) {

            recipeVm.PreparationSteps.Add(new PreparationStepViewModel
            {
                StepId = step.StepId,
                StepNumber = (uint)StepNumb++,
                Description = step.Description
            });
        }

        foreach (var ingredient in recipeVm.Ingredients) Console.WriteLine(ingredient);

        foreach (var sep in recipeVm.PreparationSteps) Console.WriteLine(sep);

        return View(recipeVm);
    }

    // POST: Recipes/Edit/5
    [Authorize(Roles = "Admin,Moderator")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RecipeViewModel viewModel, string IngredientsJson, string StepsJson,
        List<IFormFile> photoFiles, string? removedPhotos)
    {
        if (id != viewModel.Id) return NotFound();

        // First, get the existing recipe with its related data
        var existingRecipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.PreparationSteps)
            .Include(r => r.Photos)  // Include photos
            .FirstOrDefaultAsync(r => r.Id == id);

        if (existingRecipe == null) return NotFound();

        // Update preparation steps
        var newSteps = JsonConvert.DeserializeObject<List<PreparationStepViewModel>>(StepsJson);
        // Update ingredients
        var newIngredients = JsonConvert.DeserializeObject<List<IngredientViewModel>>(IngredientsJson);

        if (ModelState.IsValid)
            try
            {
                // Update basic recipe information
                existingRecipe.Title = viewModel.Title;
                existingRecipe.Summary = viewModel.Summary;
                existingRecipe.Category = viewModel.Category;
                existingRecipe.Diet = viewModel.Diet;
                existingRecipe.Published = viewModel.Published;
                

                // Remove all existing ingredients
                existingRecipe.Ingredients.Clear();

                // Add updated ingredients
                foreach (var ingredientVM in newIngredients)
                    existingRecipe.Ingredients.Add(new Ingredient
                    {
                        Name = ingredientVM.Name,
                        Quantity = ingredientVM.Quantity,
                        IsAllergen = ingredientVM.IsAllergen,
                        RecipeId = existingRecipe.Id
                    });

                // Remove all existing steps
                existingRecipe.PreparationSteps.Clear();

                // Add updated steps
                foreach (var stepVM in newSteps.OrderBy(s => s.StepNumber))
                    existingRecipe.PreparationSteps.Add(new PreparationStep
                    {
                        StepNumber = stepVM.StepNumber,
                        Description = stepVM.Description,
                        RecipeId = existingRecipe.Id
                    });

                // Handle removed photos
                if (!string.IsNullOrEmpty(removedPhotos))
                {
                    var photoIdsToRemove = removedPhotos.Split(',').Select(int.Parse);
                    var photosToRemove = existingRecipe.Photos.Where(p => photoIdsToRemove.Contains(p.Id)).ToList();
                    foreach (var photo in photosToRemove)
                    {
                        // Delete physical file
                        var filePath = Path.Combine("wwwroot", photo.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);
                        
                        
                        existingRecipe.Photos.Remove(photo);
                    }
                }

                // Handle new photo uploads
                if (photoFiles != null && photoFiles.Any())
                {
                    if (existingRecipe.Photos == null)
                        existingRecipe.Photos = new List<Photo>();

                    foreach (var file in photoFiles)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                            var filePath = Path.Combine("wwwroot", "uploads", "recipes", fileName);

                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            existingRecipe.Photos.Add(new Photo
                            {
                                ImagePath = $"/uploads/recipes/{fileName}"
                            });
                        }
                    }
                }

                _context.Update(existingRecipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(viewModel.Id))
                    return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating recipe: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the recipe. Please try again.");
            }

        // If we got this far, something failed, redisplay form
        // Repopulate the lists before returning the view
        viewModel.Ingredients = newIngredients ?? new List<IngredientViewModel>();
        viewModel.PreparationSteps = newSteps ?? new List<PreparationStepViewModel>();

        return View(viewModel);
    }

    // GET: Recipes/Delete/5
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var recipe = await _context.Recipes
            .Include(r => r.Author)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (recipe == null) return NotFound();

        return View(recipe);
    }

    // POST: Recipes/Delete/5
    [Authorize(Roles = "Admin,Moderator")]
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe != null) _context.Recipes.Remove(recipe);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool RecipeExists(int id)
    {
        return _context.Recipes.Any(e => e.Id == id);
    }
}