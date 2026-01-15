using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MyMvcApp.Authorization.Requirements;
using MyMvcApp.Models;

namespace MyMvcApp.Authorization.Handlers;

public class ProviderApprovedHandler : AuthorizationHandler<ProviderApprovedRequirement>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProviderApprovedHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProviderApprovedRequirement requirement)
    {
        var user = await _userManager.GetUserAsync(context.User);
        if (user != null && user.Role == "Provider" && user.IsApproved)
        {
            context.Succeed(requirement);
        }
    }
}





