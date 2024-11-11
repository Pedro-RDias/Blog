using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Blog
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Comments/Recipe/5
        [HttpGet("recipe/{recipeId}")]
        public async Task<ActionResult<object>> GetRecipeComments(int recipeId)
        {
            try
            {
                var comments = await _context.Comments
                    .Where(c => c.RecipeId == recipeId)  // Remove the ReplyToId condition for now
                    .Include(c => c.Author)
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();

                Console.WriteLine($"Found {comments.Count} comments for recipe {recipeId}"); // Debug logging

                var formattedComments = comments.Select(c => new
                {
                    id = c.Id,
                    content = c.Content,
                    dateCreated = c.DateCreated,
                    authorName = c.Author?.UserName ?? "Anonymous"
                }).ToList();

                return Ok(new { 
                    success = true, 
                    data = formattedComments,
                    count = formattedComments.Count
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting comments: {ex.Message}"); // Debug logging
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/Comments
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<object>> PostComment([FromBody] CommentDto commentDto)
        {
            try
            {
                if (string.IsNullOrEmpty(commentDto?.Content))
                    return BadRequest(new { success = false, message = "Comment content is required" });

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return BadRequest(new { success = false, message = "User not authenticated" });

                var comment = new Comment
                {
                    Content = commentDto.Content,
                    RecipeId = commentDto.RecipeId,
                    AuthorId = userId,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Replies = new List<Comment>()
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                // Load the author details
                await _context.Entry(comment)
                    .Reference(c => c.Author)
                    .LoadAsync();

                // Create a response object
                var response = new
                {
                    success = true,
                    data = new
                    {
                        id = comment.Id,
                        content = comment.Content,
                        dateCreated = comment.DateCreated,
                        authorName = comment.Author?.UserName
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        public class CommentDto
        {
            public string Content { get; set; }
            public int RecipeId { get; set; }
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
