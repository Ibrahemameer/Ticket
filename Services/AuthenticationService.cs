using Microsoft.AspNetCore.Identity;
using RhinoTicketingSystem.Models;

public class AuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthenticationService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> ValidateUserAccess(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsEmailConfirmedAsync(user);
    }
}