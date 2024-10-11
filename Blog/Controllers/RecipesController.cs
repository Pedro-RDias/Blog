using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;
using Blog.Data.Enums;

namespace Blog.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
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
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: Recipes/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");

            var diets = from Diet d in Enum.GetValues(typeof(Diet))
                        select new { ID = (int)d, Name = d.ToString() };

            ViewData["Diet"] = new SelectList(diets, "ID", "Name");

            var categories = from Category c in Enum.GetValues(typeof(Category))
                             select new { ID = (int)c, Name = c.ToString() };

            ViewData["Category"] = new SelectList(categories, "ID", "Name");

            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Summary,Category,Diet")] Recipe recipe)
        {

            // ADD Loged user id
            if (ModelState.IsValid)
            {
                var dNow = new DateTime();
                recipe.DateCreated = dNow;
                recipe.DateUpdated = dNow;
                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", recipe.AuthorId);
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", recipe.AuthorId);

            var diets = from Diet d in Enum.GetValues(typeof(Diet))
                        select new { ID = (int)d, Name = d.ToString() };

            ViewData["Diet"] = new SelectList(diets, "ID", "Name", recipe.Diet);

            var categories = from Category c in Enum.GetValues(typeof(Category))
                             select new { ID = (int)c, Name = c.ToString() };

            ViewData["Category"] = new SelectList(categories, "ID", "Name", recipe.Category);


            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Summary,Category,Diet,AuthorId,DateCreated,DateUpdated")] Recipe recipe)
        {
            // Check if user is admin or creator
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    recipe.DateUpdated = new DateTime();
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", recipe.AuthorId);


            var diets = from Diet d in Enum.GetValues(typeof(Diet))
                        select new { ID = (int)d, Name = d.ToString() };

            ViewData["Diet"] = new SelectList(diets, "ID", "Name", recipe.Diet);

            var categories = from Category c in Enum.GetValues(typeof(Category))
                             select new { ID = (int)c, Name = c.ToString() };

            ViewData["Category"] = new SelectList(categories, "ID", "Name", recipe.Category);

            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }
    }
}
