using Blog.Data;
using Blog.Data.Enums;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // If we got this far, something failed; redisplay form


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


        // Get Diets And Categories From Enum
        var diets = from Diet d in Enum.GetValues(typeof(Diet))
            select new { ID = (int)d, Name = d.ToString() };
        ViewData["Diet"] = new SelectList(diets, "ID", "Name", recipe.Diet);

        var categories = from Category c in Enum.GetValues(typeof(Category))
            select new { ID = (int)c, Name = c.ToString() };
        ViewData["Category"] = new SelectList(categories, "ID", "Name", recipe.Category);

        return View(recipe);
    }

    // POST: Recipes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,Title,Summary,Category,Diet,Ingredients,PreparationSteps")]
        Recipe recipe)
    {
        return View(recipe);
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


    private SelectList GetEnumSelectList<TEnum>()
    {
        var values = Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(e => new { Value = e, Text = e.ToString() })
            .ToList();

        return new SelectList(values, "Value", "Text");
    }

    private SelectList GetEnumSelectList<TEnum>(TEnum selected)
    {
        var values = Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(e => new { Value = e, Text = e.ToString() })
            .ToList();

        return new SelectList(values, "Value", "Text", selected);
    }

    private void LogModelErrors()
    {
        foreach (var modelState in ModelState.Values)
        foreach (var error in modelState.Errors)
            Console.WriteLine(error.ErrorMessage);
    }
}