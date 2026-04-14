using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public UsersController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        // If current user is blocked, sign them out
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null || currentUser.IsBlocked)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        var users = await _userManager.Users
            .OrderByDescending(u => u.LastLoginAt)
            .ToListAsync();

        ViewData["CurrentUserId"] = currentUser.Id;
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Block([FromBody] UserActionRequest request)
    {
        if (request.Ids == null || !request.Ids.Any())
            return Json(new { success = false, message = "No users selected." });

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null || currentUser.IsBlocked)
        {
            await _signInManager.SignOutAsync();
            return Json(new { success = false, redirect = Url.Action("Login", "Account") });
        }

        var users = await _userManager.Users
            .Where(u => request.Ids.Contains(u.Id))
            .ToListAsync();

        bool currentUserBlocked = false;

        foreach (var user in users)
        {
            user.IsBlocked = true;
            await _userManager.UpdateAsync(user);
            if (user.Id == currentUser.Id)
                currentUserBlocked = true;
        }

        if (currentUserBlocked)
        {
            await _signInManager.SignOutAsync();
            return Json(new { success = true, redirect = Url.Action("Login", "Account") });
        }

        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unblock([FromBody] UserActionRequest request)
    {
        if (request.Ids == null || !request.Ids.Any())
            return Json(new { success = false, message = "No users selected." });

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null || currentUser.IsBlocked)
        {
            await _signInManager.SignOutAsync();
            return Json(new { success = false, redirect = Url.Action("Login", "Account") });
        }

        var users = await _userManager.Users
            .Where(u => request.Ids.Contains(u.Id))
            .ToListAsync();

        foreach (var user in users)
        {
            user.IsBlocked = false;
            await _userManager.UpdateAsync(user);
        }

        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete([FromBody] UserActionRequest request)
    {
        if (request.Ids == null || !request.Ids.Any())
            return Json(new { success = false, message = "No users selected." });

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null || currentUser.IsBlocked)
        {
            await _signInManager.SignOutAsync();
            return Json(new { success = false, redirect = Url.Action("Login", "Account") });
        }

        bool currentUserDeleted = false;

        var users = await _userManager.Users
            .Where(u => request.Ids.Contains(u.Id))
            .ToListAsync();

        foreach (var user in users)
        {
            if (user.Id == currentUser.Id)
                currentUserDeleted = true;
            await _userManager.DeleteAsync(user);
        }

        if (currentUserDeleted)
        {
            await _signInManager.SignOutAsync();
            return Json(new { success = true, redirect = Url.Action("Login", "Account") });
        }

        return Json(new { success = true });
    }
}

public class UserActionRequest
{
    public List<string>? Ids { get; set; }
}
