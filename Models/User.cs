using Microsoft.AspNetCore.Identity;

namespace Blog.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}