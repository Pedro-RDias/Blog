using Blog.Models;
using Humanizer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add DbSet properties here
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<PreparationStep> PreparationSteps { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Recipe>()
                .HasMany(r => r.Ingredients)
                .WithOne(i => i.Recipe)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Recipe>()
                .HasMany(r => r.PreparationSteps)
                .WithOne(p => p.Recipe)
                .HasForeignKey(p => p.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Recipe>()
                .HasMany(r => r.Comments)
                .WithOne(c => c.Recipe)
                .HasForeignKey(c => c.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Recipe>()
                .HasMany(r => r.Ratings)
                .WithOne(r => r.Recipe)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
            
            base.OnModelCreating(builder);
        }
    }
}
