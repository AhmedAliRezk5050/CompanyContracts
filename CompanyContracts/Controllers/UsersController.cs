using System.Security.Claims;
using Core.Entities;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyContracts.Models;

namespace CompanyContracts.Controllers;

public class UsersController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public UsersController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var userDetailsList = await _userManager
                .Users
                .Select(u => new UserDetails
                {
                    Id = u.Id,
                    UserName = u.UserName
                })
                .ToListAsync();

            return View(userDetailsList);
        }
        catch (Exception e)
        {
            TempData["home-temp-msg"] = "unknown-error";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    [Authorize(Permissions.Claims.Edit)]
    public async Task<IActionResult> ManageUserPermissions(string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);
            
            if (user is null)
            {
                return NotFound();
            }

            var allClaims = Permissions.GetAllPermissions();
            var userClaims = (await _userManager.GetClaimsAsync(user)).Select(c => c.Value).ToList();

            var viewModel = new UserClaimsViewModel
            {
                Id = userId,
                ClaimStatusViewModels = allClaims.Select(claim => new ClaimStatusViewModel
                {
                    Value = claim,
                    Status = userClaims.Contains(claim)
                }).ToList()
            };

            return View(viewModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [HttpPost]
    [Authorize(Permissions.Claims.Edit)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageUserPermissions(UserClaimsViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
        {
            return NotFound();
        }

        // Remove all user's current claims
        var currentClaims = await _userManager.GetClaimsAsync(user);
        await _userManager.RemoveClaimsAsync(user, currentClaims);

        // Add claims based on the checkboxes that were checked
        var newClaims = model.ClaimStatusViewModels
            .Where(c => c.Status)
            .Select(c => new Claim("Permission", c.Value)) 
            .ToList();

        await _userManager.AddClaimsAsync(user, newClaims);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Account/Login", new { area = "Identity" });
    }
    
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
        
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        
            if (!changePasswordResult.Succeeded)
            {
                TempData["home-temp-msg"] = "unknown-error";
                return RedirectToAction("Index", "Home");
            }

            await _signInManager.RefreshSignInAsync(user);
            
            TempData["home-temp-msg"] = "change-password-success";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Permissions.Users.Edit)]
    public async Task<IActionResult> AdminChangePassword(AdminChangePasswordViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        
        if (user == null)
        {
            return NotFound();
        }

        if (model.NewPassword != model.ConfirmPassword)
        {
            return RedirectToAction(nameof(Index));
        }

        // Create a new password hash
        var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);

        // Set and save the new password
        user.PasswordHash = newPasswordHash;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
    }
}