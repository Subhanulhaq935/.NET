using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Data;
using MyMvcApp.Models;
using MyMvcApp.Repositories;
using MyMvcApp.Services;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MyMvcApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBookingRepository _bookingRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICookieService _cookieService;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        IBookingRepository bookingRepository,
        IServiceRepository serviceRepository,
        IReviewRepository reviewRepository,
        ICategoryRepository categoryRepository,
        ICookieService cookieService)
    {
        _userManager = userManager;
        _bookingRepository = bookingRepository;
        _serviceRepository = serviceRepository;
        _reviewRepository = reviewRepository;
        _categoryRepository = categoryRepository;
        _cookieService = cookieService;
        QuestPDF.Settings.License = LicenseType.Community;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<IActionResult> Index()
    {
        var allUsers = _userManager.Users.ToList();
        var totalUsers = allUsers.Count;
        var totalProviders = allUsers.Count(u => u.Role == "Provider");
        var totalCustomers = allUsers.Count(u => u.Role == "Customer");
        var pendingProviders = allUsers.Count(u => u.Role == "Provider" && !u.IsApproved);

        var allBookings = await _bookingRepository.GetAllAsync();
        var totalBookings = allBookings.Count();
        var recentBookings = allBookings.OrderByDescending(b => b.CreatedAt).Take(10).ToList();

        var allServices = await _serviceRepository.GetAllAsync();
        var totalServices = allServices.Count();

        var allReviews = await _reviewRepository.GetAllAsync();
        var totalReviews = allReviews.Count();

        // Booking statistics by status
        var bookingStats = allBookings
            .GroupBy(b => b.Status)
            .Select(g => new { status = g.Key, count = g.Count() })
            .ToList();

        ViewBag.TotalUsers = totalUsers;
        ViewBag.TotalProviders = totalProviders;
        ViewBag.TotalCustomers = totalCustomers;
        ViewBag.TotalBookings = totalBookings;
        ViewBag.PendingProviders = pendingProviders;
        ViewBag.TotalServices = totalServices;
        ViewBag.TotalReviews = totalReviews;
        ViewBag.RecentBookings = recentBookings;
        ViewBag.BookingStats = bookingStats;

        // Store admin dashboard visit
        _cookieService.SetCookie("AdminLastVisit", DateTime.UtcNow.ToString(), 1);

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ManageUsers()
    {
        var users = _userManager.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToList();

        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> ManageProviders()
    {
        var providers = _userManager.Users
            .Where(u => u.Role == "Provider")
            .ToList();

        // Load services for each provider
        foreach (var provider in providers)
        {
            var services = await _serviceRepository.GetByProviderIdAsync(provider.Id);
            // Note: Services navigation property won't work the same way in MongoDB
        }

        return View(providers);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveProvider(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && user.Role == "Provider")
        {
            user.IsApproved = true;
            await _userManager.UpdateAsync(user);
            TempData["Success"] = "Provider approved successfully!";
        }
        return RedirectToAction("ManageProviders");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisableProvider(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.IsApproved = false;
            await _userManager.UpdateAsync(user);
            TempData["Success"] = "Provider disabled successfully!";
        }
        return RedirectToAction("ManageProviders");
    }

    [HttpGet]
    public async Task<IActionResult> ManageCategories()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return View(categories);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(string name, string? description)
    {
        var category = new Category
        {
            Name = name,
            Description = description
        };
        await _categoryRepository.CreateAsync(category);
        TempData["Success"] = "Category added successfully!";
        return RedirectToAction("ManageCategories");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(int id, string name, string? description)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        category.Name = name;
        category.Description = description;
        await _categoryRepository.UpdateAsync(category);
        TempData["Success"] = "Category updated successfully!";
        return RedirectToAction("ManageCategories");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _categoryRepository.DeleteAsync(id);
        TempData["Success"] = "Category deleted successfully!";
        return RedirectToAction("ManageCategories");
    }

    [HttpGet]
    public async Task<IActionResult> AllBookings()
    {
        var bookings = await _bookingRepository.GetAllAsync();
        return View(bookings.OrderByDescending(b => b.CreatedAt));
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var bookings = await _bookingRepository.GetAllAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Bookings");

        worksheet.Cells[1, 1].Value = "Booking ID";
        worksheet.Cells[1, 2].Value = "Service";
        worksheet.Cells[1, 3].Value = "Customer";
        worksheet.Cells[1, 4].Value = "Provider";
        worksheet.Cells[1, 5].Value = "Date/Time";
        worksheet.Cells[1, 6].Value = "Status";
        worksheet.Cells[1, 7].Value = "Created At";

        int row = 2;
        foreach (var booking in bookings)
        {
            worksheet.Cells[row, 1].Value = booking.BookingId;
            worksheet.Cells[row, 2].Value = booking.Service.Title;
            worksheet.Cells[row, 3].Value = booking.Customer.Name;
            worksheet.Cells[row, 4].Value = booking.Provider.Name;
            worksheet.Cells[row, 5].Value = booking.BookingDateTime;
            worksheet.Cells[row, 6].Value = booking.Status;
            worksheet.Cells[row, 7].Value = booking.CreatedAt;
            row++;
        }

        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"Bookings_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportPDF()
    {
        var bookings = await _bookingRepository.GetAllAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.Header().Text("SkillHub - Bookings Report").FontSize(20).Bold();
                page.Content().Column(column =>
                {
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("ID");
                            header.Cell().Element(CellStyle).Text("Service");
                            header.Cell().Element(CellStyle).Text("Customer");
                            header.Cell().Element(CellStyle).Text("Provider");
                            header.Cell().Element(CellStyle).Text("Status");
                        });

                        foreach (var booking in bookings)
                        {
                            table.Cell().Element(CellStyle).Text(booking.BookingId.ToString());
                            table.Cell().Element(CellStyle).Text(booking.Service.Title);
                            table.Cell().Element(CellStyle).Text(booking.Customer.Name);
                            table.Cell().Element(CellStyle).Text(booking.Provider.Name);
                            table.Cell().Element(CellStyle).Text(booking.Status);
                        }
                    });
                });
            });
        });

        var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;

        return File(stream, "application/pdf", $"Bookings_{DateTime.Now:yyyyMMdd}.pdf");
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container
            .Border(1)
            .Padding(5)
            .AlignMiddle();
    }
}

