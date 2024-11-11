using System.Diagnostics;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

public class RecipesViewIndexModel
{
    public PaginatedList<Recipe> Recipes { get; set; }
    public Dictionary<int, List<int>> Dates { get; set; }
    public string SearchTerm { get; set; }
    public int? CurrentMonth { get; set; }
    public int? CurrentYear { get; set; }
    public RecipeFilterParameters FilterParams { get; set; }
}

public class RecipeFilterParameters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public string SearchTerm { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
}

// PaginatedList.cs
public class PaginatedList<T> : List<T>
{
    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalItems = count;
        AddRange(items);
    }

    public int PageIndex { get; }
    public int TotalPages { get; }
    public int TotalItems { get; private set; }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize)
            .Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(RecipeFilterParameters parameters)
    {
        parameters.PageSize = 8; // 8 receitas por página
        
        var query = _context.Recipes
            .Include(r => r.Author)
            .Include(r => r.Comments)
            .Include(r => r.Ratings)
            .Include(r => r.Ingredients)
            .Include(r => r.PreparationSteps)
            .Include(r => r.Photos)
            .Where(r => r.Published) // Add this line to filter published recipes
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(parameters.SearchTerm))
            query = query.Where(r =>
                r.Title.Contains(parameters.SearchTerm) ||
                r.Summary.Contains(parameters.SearchTerm));

        // Apply date filters
        if (parameters.Month.HasValue && parameters.Year.HasValue)
            query = query.Where(r =>
                r.DateCreated.Month == parameters.Month &&
                r.DateCreated.Year == parameters.Year);

        // Apply sorting
        query = query.OrderByDescending(r => r.DateCreated);

        // Get dates for sidebar
        var allRecipes = await _context.Recipes.ToListAsync();
        var dates = new Dictionary<int, List<int>>();
        foreach (var recipe in allRecipes)
        {
            if (!dates.ContainsKey(recipe.DateCreated.Year)) dates[recipe.DateCreated.Year] = new List<int>();

            if (!dates[recipe.DateCreated.Year].Contains(recipe.DateCreated.Month))
                dates[recipe.DateCreated.Year].Add(recipe.DateCreated.Month);
        }

        // Create paginated list
        var recipes = await PaginatedList<Recipe>.CreateAsync(
            query, parameters.PageNumber, parameters.PageSize);

        var viewModel = new RecipesViewIndexModel
        {
            Recipes = recipes,
            Dates = dates,
            SearchTerm = parameters.SearchTerm,
            CurrentMonth = parameters.Month,
            CurrentYear = parameters.Year,
            FilterParams = parameters
        };

        return View(viewModel);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }
    
}