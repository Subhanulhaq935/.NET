using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MyMvcApp.Authorization.Requirements;
using MyMvcApp.Models;
using MyMvcApp.Repositories;

namespace MyMvcApp.Authorization.Handlers;

public class BookingAccessHandler : AuthorizationHandler<BookingAccessRequirement, int>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBookingRepository _bookingRepository;

    public BookingAccessHandler(
        UserManager<ApplicationUser> userManager,
        IBookingRepository bookingRepository)
    {
        _userManager = userManager;
        _bookingRepository = bookingRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BookingAccessRequirement requirement,
        int bookingId)
    {
        var user = await _userManager.GetUserAsync(context.User).ConfigureAwait(false);
        if (user == null) return;

        // Admins always have access
        if (await _userManager.IsInRoleAsync(user, "Admin").ConfigureAwait(false))
        {
            context.Succeed(requirement);
            return;
        }

        var booking = await _bookingRepository.GetByIdAsync(bookingId).ConfigureAwait(false);
        if (booking == null) return;

        // Check if user is the customer or provider of the booking
        if (booking.CustomerId == user.Id || booking.ProviderId == user.Id)
        {
            context.Succeed(requirement);
        }
    }
}





