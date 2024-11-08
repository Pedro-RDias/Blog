using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;
using System.Security.Claims;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{recipeId}")]
        public async Task<ActionResult<double>> GetRecipeRating(int recipeId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.RecipeId == recipeId)
                .Select(r => r.RatingValue)
                .ToListAsync();

            if (!ratings.Any())
                return 0;

            return ratings.Average();
        }

        [HttpPost]
        public async Task<IActionResult> PostRating([FromBody] RatingRequest request)
        {
            if (request == null || request.RecipeId == 0 || request.RatingValue < 1 || request.RatingValue > 5)
                return BadRequest("Invalid rating data");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var rating = new Rating
            {
                RecipeId = request.RecipeId,
                RatingValue = request.RatingValue,
                UserId = userId,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };

            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.RecipeId == rating.RecipeId && r.UserId == userId);

            if (existingRating != null)
            {
                existingRating.RatingValue = rating.RatingValue;
                existingRating.DateUpdated = DateTime.Now;
            }
            else
            {
                _context.Ratings.Add(rating);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        public class RatingRequest
        {
            public int RecipeId { get; set; }
            public int RatingValue { get; set; }
        }
    }
}
