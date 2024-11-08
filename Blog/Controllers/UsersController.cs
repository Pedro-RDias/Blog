using Blog.Areas.Identity.Data;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegisterModel = Blog.Areas.Identity.Pages.Account.RegisterModel;
using Microsoft.AspNetCore.Authorization;  // Add this using statement

namespace Blog.Controllers;

public class UserRoleViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<RoleViewModel> Roles { get; set; }
}

public class RoleViewModel
{
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public bool IsSelected { get; set; }
}

[Authorize(Roles = "Admin")]  // Add this attribute
public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _context.Users.ToListAsync();
        return View(users);
    }


    public async Task<IActionResult> Details(string? id)
    {
       if (id == null) return NotFound();

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null) return NotFound();

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);
        ViewBag.UserRoles = roles;

        return View(user);
    }

    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegisterModel.InputModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Moderator);
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

      public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Get all roles
        var roles = _roleManager.Roles.ToList();
        // Get user's current roles
        var userRoles = await _userManager.GetRolesAsync(user);

        var roleViewModels = roles.Select(role => new RoleViewModel
        {
            RoleId = role.Id,
            RoleName = role.Name,
            IsSelected = userRoles.Contains(role.Name)
        }).ToList();

        var viewModel = new UserRoleViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Roles = roleViewModels
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? id, UserRoleViewModel model)
    {
        if (id == null || id != model.UserId)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var selectedRoles = model.Roles.Where(x => x.IsSelected).Select(x => x.RoleName);
        var rolesToAdd = selectedRoles.Except(userRoles);
        var rolesToRemove = userRoles.Except(selectedRoles);

        try
        {
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            await _userManager.AddToRolesAsync(user, rolesToAdd);

            TempData["Success"] = "User roles updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "An error occurred while updating roles.");
            return View(model);
        }
    }

    private bool UserExists(string id)
    {
        return _context.Users.Any(e => e.Id == id);
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null) return NotFound();

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null) return NotFound();

        return View(user);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _context.Users.FindAsync(id);
        _context.Users.Remove(user);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}