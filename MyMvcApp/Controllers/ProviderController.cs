using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Authorization.Attributes;
using MyMvcApp.Models;
using MyMvcApp.Repositories;
using MyMvcApp.Services;

namespace MyMvcApp.Controllers;

[Authorize(Roles = "Provider", Policy = "ProviderApproved")]
public class ProviderController : Controller
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICookieService _cookieService;

    public ProviderController(
        IServiceRepository serviceRepository,
        IBookingRepository bookingRepository,
        IReviewRepository reviewRepository,
        IAvailabilityRepository availabilityRepository,
        ICategoryRepository categoryRepository,
        UserManager<ApplicationUser> userManager,
        ICookieService cookieService)
    {
        _serviceRepository = serviceRepository;
        _bookingRepository = bookingRepository;
        _reviewRepository = reviewRepository;
        _availabilityRepository = availabilityRepository;
        _categoryRepository = categoryRepository;
        _userManager = userManager;
        _cookieService = cookieService;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
        if (user == null) return NotFound();

        var services = await _serviceRepository.GetByProviderIdAsync(user.Id).ConfigureAwait(false);
        var allBookings = await _bookingRepository.GetByProviderIdAsync(user.Id).ConfigureAwait(false);
        
        var upcomingBookings = allBookings
            .Where(b => (b.Status == "Pending" || b.Status == "Accepted") && b.BookingDateTime >= DateTime.Now)
            .OrderBy(b => b.BookingDateTime)
            .ToList();

        var reviews = await _reviewRepository.GetByProviderIdAsync(user.Id).ConfigureAwait(false);
        var averageRating = await _reviewRepository.GetAverageRatingByProviderIdAsync(user.Id).ConfigureAwait(false);

        ViewBag.Services = services;
        ViewBag.UpcomingBookings = upcomingBookings;
        ViewBag.Reviews = reviews.Take(10);
        ViewBag.AverageRating = averageRating;

        // Store dashboard visit
        _cookieService.SetCookie("ProviderLastVisit", DateTime.UtcNow.ToString(), 1);

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ManageServices()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var services = await _serviceRepository.GetByProviderIdAsync(user.Id);
        return View(services);
    }

    [HttpGet]
    public async Task<IActionResult> AddService()
    {
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = categories;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddService(Service service)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["Error"] = "User not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        // Check if categories exist
        var categories = await _categoryRepository.GetAllAsync();
        if (!categories.Any())
        {
            ModelState.AddModelError("", "No categories available. Please contact administrator to add categories.");
            ViewBag.Categories = categories;
            return View(service);
        }

        // Validate CategoryId is selected and exists
        if (service.CategoryId <= 0)
        {
            ModelState.AddModelError("CategoryId", "Please select a category.");
        }
        else if (!categories.Any(c => c.CategoryId == service.CategoryId))
        {
            ModelState.AddModelError("CategoryId", "Selected category is invalid.");
        }

        // Remove Provider navigation property from validation (it's set automatically)
        ModelState.Remove("Provider");
        ModelState.Remove("Category");

        if (ModelState.IsValid)
        {
            try
            {
                service.ProviderId = user.Id;
                service.CreatedAt = DateTime.UtcNow;
                service.IsActive = true;

                var serviceId = await _serviceRepository.CreateAsync(service);

                TempData["Success"] = "Service added successfully!";
                return RedirectToAction("ManageServices");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding service: {ex.Message}");
                ViewBag.Categories = categories;
                return View(service);
            }
        }

        ViewBag.Categories = categories;
        return View(service);
    }

    [HttpGet]
    [AuthorizeResourceOwner("id")]
    public async Task<IActionResult> EditService(int id)
    {
        var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
        if (user == null) return NotFound();

        var service = await _serviceRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (service == null) return NotFound();

        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = categories;
        return View(service);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizeResourceOwner("id")]
    public async Task<IActionResult> EditService(int id, Service service)
    {
        var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
        if (user == null) return NotFound();

        if (id != service.ServiceId)
        {
            return NotFound();
        }

        var existingService = await _serviceRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (existingService == null) return NotFound();

        if (ModelState.IsValid)
        {
            existingService.Title = service.Title;
            existingService.Description = service.Description;
            existingService.Price = service.Price;
            existingService.CategoryId = service.CategoryId;
            existingService.Location = service.Location;
            existingService.IsActive = service.IsActive;

            await _serviceRepository.UpdateAsync(existingService);
            TempData["Success"] = "Service updated successfully!";
            return RedirectToAction("ManageServices");
        }

        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = categories;
        return View(service);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteService(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var service = await _serviceRepository.GetByIdAsync(id);
        if (service == null || service.ProviderId != user.Id) return NotFound();

        // Check if service has any bookings
        var bookings = await _bookingRepository.GetByServiceIdAsync(id);
        if (bookings.Any())
        {
            var bookingCount = bookings.Count();
            TempData["Error"] = $"Cannot delete service. This service has {bookingCount} active booking(s). Please cancel or complete all bookings before deleting the service.";
            return RedirectToAction("ManageServices");
        }

        await _serviceRepository.DeleteAsync(id);

        TempData["Success"] = "Service deleted successfully!";
        return RedirectToAction("ManageServices");
    }

    [HttpGet]
    public async Task<IActionResult> ManageBookings()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var bookings = await _bookingRepository.GetByProviderIdAsync(user.Id);
        return View(bookings.OrderByDescending(b => b.BookingDateTime).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBookingStatus(int bookingId, string status)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null || booking.ProviderId != user.Id) return NotFound();

        if (status == "Accepted" || status == "Rejected" || status == "Completed" || status == "Cancelled")
        {
            var newStatus = status == "Rejected" ? "Cancelled" : status;
            
            // Don't allow cancellation if booking time has passed
            if (newStatus == "Cancelled" && booking.BookingDateTime < DateTime.Now && booking.Status == "Completed")
            {
                TempData["Error"] = "Cannot cancel a completed booking.";
                return RedirectToAction("ManageBookings");
            }

            await _bookingRepository.UpdateStatusAsync(bookingId, newStatus);
            
            var statusMessage = newStatus switch
            {
                "Accepted" => "Booking accepted successfully!",
                "Cancelled" => "Booking cancelled successfully!",
                "Completed" => "Booking marked as completed!",
                _ => "Booking status updated!"
            };
            
            TempData["Success"] = statusMessage;
        }

        return RedirectToAction("ManageBookings");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null || booking.ProviderId != user.Id) return NotFound();

        // Only allow cancellation if booking is Pending or Accepted
        if (booking.Status != "Pending" && booking.Status != "Accepted")
        {
            TempData["Error"] = "Only pending or accepted bookings can be cancelled.";
            return RedirectToAction("ManageBookings");
        }

        await _bookingRepository.UpdateStatusAsync(bookingId, "Cancelled");

        TempData["Success"] = "Booking cancelled successfully!";
        return RedirectToAction("ManageBookings");
    }

    [HttpGet]
    public async Task<IActionResult> Availability()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var availabilities = await _availabilityRepository.GetByProviderIdAsync(user.Id);
        return View(availabilities);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAvailability(DateTime date, TimeSpan startTime, TimeSpan endTime)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        if (date < DateTime.Today)
        {
            TempData["Error"] = "Date must be today or in the future.";
            return RedirectToAction("Availability");
        }

        var availability = new Availability
        {
            ProviderId = user.Id,
            Date = date.Date,
            StartTime = startTime,
            EndTime = endTime,
            IsAvailable = true
        };

        await _availabilityRepository.CreateAsync(availability);

        TempData["Success"] = "Availability added successfully!";
        return RedirectToAction("Availability");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAvailability(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var availability = await _availabilityRepository.GetByIdAsync(id);
        if (availability != null && availability.ProviderId == user.Id)
        {
            await _availabilityRepository.DeleteAsync(id);
        }

        return RedirectToAction("Availability");
    }
}
