using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

    public IActionResult Create()
    {
        return View(new RecipeViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RecipeViewModel viewModel, string IngredientsJson, string StepsJson)
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
                AuthorId = _userManager.GetUserId(User),
                Ingredients = ingredients.Select(i => new Ingredient
                {
                    Name = i.Name,
                    Quantity = i.Quantity,
                    IsAllergen = i.IsAllergen
                }).ToList(),
                PreparationSteps = steps.Select(s => new PreparationStep
                {
                    StepNumber = (uint)s.StepNumber,
                    Description = s.Description
                }).ToList()
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        viewModel.Ingredients = ingredients;
        viewModel.PreparationSteps = steps;

        return View(viewModel);
    }

    // GET: Recipes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.PreparationSteps)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return NotFound();

        var recipeVm = new RecipeViewModel
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Summary = recipe.Summary,
            Category = recipe.Category,
            Diet = recipe.Diet,
        };

        foreach (var ingredient in recipe.Ingredients)
        {
            recipeVm.Ingredients.Add(new IngredientViewModel
            {
                IngredientId = ingredient.IngredientId,
                Name = ingredient.Name,
                Quantity = ingredient.Quantity,
                IsAllergen = ingredient.IsAllergen,
            });
        }

        foreach (var step in recipe.PreparationSteps)
        {
            recipeVm.PreparationSteps.Add(new PreparationStepViewModel
            {
                StepId = step.StepId,
                StepNumber = step.StepNumber,
                Description = step.Description,
            });
        }

        foreach (var ingredient in recipeVm.Ingredients)
        {
            Console.WriteLine(ingredient);
        }

        foreach (var sep in recipeVm.PreparationSteps)
        {
            Console.WriteLine(sep);
        }

        return View(recipeVm);
    }

    // POST: Recipes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RecipeViewModel viewModel, string IngredientsJson, string StepsJson)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
        }

        // First, get the existing recipe with its related data
        var existingRecipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.PreparationSteps)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (existingRecipe == null)
        {
            return NotFound();
        }

        // Update preparation steps
        var newSteps = JsonConvert.DeserializeObject<List<PreparationStepViewModel>>(StepsJson);
        // Update ingredients
        var newIngredients = JsonConvert.DeserializeObject<List<IngredientViewModel>>(IngredientsJson);

        if (ModelState.IsValid)
        {
            try
            {
                // Update basic recipe information
                existingRecipe.Title = viewModel.Title;
                existingRecipe.Summary = viewModel.Summary;
                existingRecipe.Category = viewModel.Category;
                existingRecipe.Diet = viewModel.Diet;
                
                // Remove all existing ingredients
                existingRecipe.Ingredients.Clear();

                // Add updated ingredients
                foreach (var ingredientVM in newIngredients)
                {
                    existingRecipe.Ingredients.Add(new Ingredient
                    {
                        Name = ingredientVM.Name,
                        Quantity = ingredientVM.Quantity,
                        IsAllergen = ingredientVM.IsAllergen,
                        RecipeId = existingRecipe.Id
                    });
                }
                
                // Remove all existing steps
                existingRecipe.PreparationSteps.Clear();

                // Add updated steps
                foreach (var stepVM in newSteps.OrderBy(s => s.StepNumber))
                {
                    existingRecipe.PreparationSteps.Add(new PreparationStep
                    {
                        StepNumber = stepVM.StepNumber,
                        Description = stepVM.Description,
                        RecipeId = existingRecipe.Id
                    });
                }

                _context.Update(existingRecipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(viewModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating recipe: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the recipe. Please try again.");
            }
        }

        // If we got this far, something failed, redisplay form
        // Repopulate the lists before returning the view
        viewModel.Ingredients = newIngredients ?? new List<IngredientViewModel>();
        viewModel.PreparationSteps = newSteps ?? new List<PreparationStepViewModel>();

        return View(viewModel);
    }
    
    // GET: Recipes/Delete/5
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