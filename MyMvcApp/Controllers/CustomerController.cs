using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Models;
using MyMvcApp.Repositories;
using MyMvcApp.Services;

namespace MyMvcApp.Controllers;

[Authorize(Roles = "Customer")]
public class CustomerController : Controller
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICookieService _cookieService;

    public CustomerController(
        IBookingRepository bookingRepository,
        IServiceRepository serviceRepository,
        IAvailabilityRepository availabilityRepository,
        IReviewRepository reviewRepository,
        UserManager<ApplicationUser> userManager,
        ICookieService cookieService)
    {
        _bookingRepository = bookingRepository;
        _serviceRepository = serviceRepository;
        _availabilityRepository = availabilityRepository;
        _reviewRepository = reviewRepository;
        _userManager = userManager;
        _cookieService = cookieService;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var allBookings = await _bookingRepository.GetByCustomerIdAsync(user.Id);
        var upcomingBookings = allBookings
            .Where(b => (b.Status == "Pending" || b.Status == "Accepted") && b.BookingDateTime >= DateTime.Now)
            .OrderBy(b => b.BookingDateTime)
            .ToList();

        var pastBookings = allBookings
            .Where(b => b.Status == "Completed" || b.Status == "Cancelled" || b.BookingDateTime < DateTime.Now)
            .OrderByDescending(b => b.BookingDateTime)
            .Take(10)
            .ToList();

        // Load reviews for past bookings
        foreach (var booking in pastBookings)
        {
            booking.Review = await _reviewRepository.GetByBookingIdAsync(booking.BookingId);
        }

        ViewBag.UpcomingBookings = upcomingBookings;
        ViewBag.PastBookings = pastBookings;

        // Store dashboard visit in cookie
        _cookieService.SetCookie("LastDashboardVisit", DateTime.UtcNow.ToString(), 1);

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Book(int serviceId)
    {
        var service = await _serviceRepository.GetByIdWithDetailsAsync(serviceId);
        if (service == null) return NotFound();

        var availabilities = await _availabilityRepository.GetAvailableSlotsAsync(service.ProviderId);

        ViewBag.Availabilities = availabilities;
        return View(service);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int serviceId, DateTime bookingDateTime)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        // Get service with provider details
        var service = await _serviceRepository.GetByIdWithDetailsAsync(serviceId);
        if (service == null) return NotFound();

        if (bookingDateTime < DateTime.Now)
        {
            ModelState.AddModelError("", "Booking date must be in the future.");
            return RedirectToAction("Book", new { serviceId });
        }

        var booking = new Booking
        {
            ServiceId = serviceId,
            CustomerId = user.Id,
            ProviderId = service.ProviderId,
            BookingDateTime = bookingDateTime,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _bookingRepository.CreateAsync(booking);

        // Store booking in cookie for tracking
        _cookieService.SetCookie("LastBookingId", booking.BookingId.ToString(), 7);

        TempData["Success"] = "Booking request submitted successfully!";
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize(Policy = "BookingAccess")]
    public async Task<IActionResult> Review(int bookingId)
    {
        var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
        if (user == null) return NotFound();

        var booking = await _bookingRepository.GetByIdAsync(bookingId).ConfigureAwait(false);
        if (booking == null || booking.Status != "Completed")
        {
            return NotFound();
        }

        ViewBag.Booking = booking;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Review(int bookingId, int rating, string? comment)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null || booking.CustomerId != user.Id || booking.Status != "Completed")
        {
            return NotFound();
        }

        if (rating < 1 || rating > 5)
        {
            ModelState.AddModelError("", "Rating must be between 1 and 5.");
            return View();
        }

        var review = new Review
        {
            BookingId = bookingId,
            CustomerId = user.Id,
            Rating = rating,
            Comment = comment,
            Date = DateTime.UtcNow
        };

        await _reviewRepository.CreateAsync(review);

        TempData["Success"] = "Review submitted successfully!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null || booking.CustomerId != user.Id) return NotFound();

        // Only allow cancellation if booking is Pending or Accepted
        if (booking.Status != "Pending" && booking.Status != "Accepted")
        {
            TempData["Error"] = "Only pending or accepted bookings can be cancelled.";
            return RedirectToAction("Index");
        }

        // Don't allow cancellation if booking time has passed
        if (booking.BookingDateTime < DateTime.Now)
        {
            TempData["Error"] = "Cannot cancel a booking that has already passed.";
            return RedirectToAction("Index");
        }

        await _bookingRepository.UpdateStatusAsync(bookingId, "Cancelled");

        TempData["Success"] = "Booking cancelled successfully!";
        return RedirectToAction("Index");
    }
}

