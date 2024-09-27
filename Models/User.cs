using Microsoft.AspNetCore.Identity;

namespace Blog.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public virtual ICollection<Recipe> Recipes { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
    public virtual ICollection<Rating> Ratings { get; set; } 
}