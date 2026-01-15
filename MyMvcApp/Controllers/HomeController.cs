using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Models;
using MyMvcApp.Repositories;
using MyMvcApp.Services;

namespace MyMvcApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IServiceRepository _serviceRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ICookieService _cookieService;

    public HomeController(
        ILogger<HomeController> logger,
        IServiceRepository serviceRepository,
        ICategoryRepository categoryRepository,
        IReviewRepository reviewRepository,
        ICookieService cookieService)
    {
        _logger = logger;
        _serviceRepository = serviceRepository;
        _categoryRepository = categoryRepository;
        _reviewRepository = reviewRepository;
        _cookieService = cookieService;
    }

    public async Task<IActionResult> Index(string? category, string? search)
    {
        // Store search filters in cookie for state management
        if (!string.IsNullOrEmpty(category))
            _cookieService.SetCookie("LastCategoryFilter", category, 7);
        if (!string.IsNullOrEmpty(search))
            _cookieService.SetCookie("LastSearchTerm", search, 7);

        var services = await _serviceRepository.SearchAsync(category, search);
        var categories = await _categoryRepository.GetAllAsync();

        ViewBag.Categories = categories;
        ViewBag.SelectedCategory = category;
        ViewBag.Search = search;

        return View(services);
    }

    public async Task<IActionResult> ServiceDetails(int id)
    {
        var service = await _serviceRepository.GetByIdWithDetailsAsync(id);

        if (service == null)
        {
            return NotFound();
        }

        var reviews = await _reviewRepository.GetByServiceIdAsync(id);
        var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

        // Store viewed service in cookie
        _cookieService.SetCookie("LastViewedService", id.ToString(), 1);

        ViewBag.Reviews = reviews;
        ViewBag.AverageRating = averageRating;

        return View(service);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
