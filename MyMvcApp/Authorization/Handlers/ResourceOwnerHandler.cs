using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MyMvcApp.Authorization.Requirements;
using MyMvcApp.Models;
using MyMvcApp.Repositories;

namespace MyMvcApp.Authorization.Handlers;

public class ResourceOwnerHandler : AuthorizationHandler<ResourceOwnerRequirement, int>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IServiceRepository _serviceRepository;
    private readonly IBookingRepository _bookingRepository;

    public ResourceOwnerHandler(
        UserManager<ApplicationUser> userManager,
        IServiceRepository serviceRepository,
        IBookingRepository bookingRepository)
    {
        _userManager = userManager;
        _serviceRepository = serviceRepository;
        _bookingRepository = bookingRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOwnerRequirement requirement,
        int resourceId)
    {
        var user = await _userManager.GetUserAsync(context.User).ConfigureAwait(false);
        if (user == null) return;

        // Check if user is Admin - admins can access any resource
        if (await _userManager.IsInRoleAsync(user, "Admin").ConfigureAwait(false))
        {
            context.Succeed(requirement);
            return;
        }

        // Check service ownership
        var service = await _serviceRepository.GetByIdAsync(resourceId).ConfigureAwait(false);
        if (service != null && service.ProviderId == user.Id)
        {
            context.Succeed(requirement);
            return;
        }

        // Check booking ownership (customer or provider)
        var booking = await _bookingRepository.GetByIdAsync(resourceId).ConfigureAwait(false);
        if (booking != null && (booking.CustomerId == user.Id || booking.ProviderId == user.Id))
        {
            context.Succeed(requirement);
        }
    }
}





