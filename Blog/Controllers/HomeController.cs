using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Blog.Data;
using Blog.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var recipes = await _context.Recipes
                .Include(r => r.Author)
                .Include(r => r.Comments)
                .Include(r => r.Ratings)
                .Include(r => r.Ingredients)
                .Include(r => r.PreparationSteps)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();
            
            var dates = new Dictionary<int, List<int>>();
            foreach (var recipe in recipes)
            {
                if (!dates.ContainsKey(recipe.DateCreated.Year))
                {
                    dates[recipe.DateCreated.Year] = new List<int>();
                }

                if (!dates[recipe.DateCreated.Year].Contains(recipe.DateCreated.Month))
                {
                    dates[recipe.DateCreated.Year].Add(recipe.DateCreated.Month);
                }
            }
            
            var viewModel = new RecipesViewIndexModel()
            {
                Recipes = recipes,
                Dates = dates
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
    }
}
