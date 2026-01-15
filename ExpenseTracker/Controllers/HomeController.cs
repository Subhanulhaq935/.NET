using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.DTOs.Report;
using ExpenseTracker.Services;

namespace ExpenseTracker.Controllers;

/// <summary>
/// Controller for home/dashboard operations
/// </summary>
[Authorize]
public class HomeController : Controller
{
    private readonly IExpenseService _expenseService;
    private readonly IReportService _reportService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IExpenseService expenseService,
        IReportService reportService,
        ILogger<HomeController> logger)
    {
        _expenseService = expenseService;
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current user's ID from claims
    /// </summary>
    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    /// <summary>
    /// Displays the dashboard with expense summary
    /// GET /Home/Index
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = GetCurrentUserId();
            var currentDate = DateTime.Now;

            // Get current month summary
            var monthlySummary = await _reportService.GetMonthlySummaryAsync(
                userId, 
                currentDate.Year, 
                currentDate.Month);

            // Get all expenses for current month
            var expenses = await _expenseService.GetExpensesByMonthAsync(
                userId, 
                currentDate.Year, 
                currentDate.Month);

            ViewBag.TotalExpenses = expenses.Count;
            ViewBag.TotalAmount = monthlySummary.TotalAmount;
            ViewBag.MonthName = new DateTime(currentDate.Year, currentDate.Month, 1).ToString("MMMM yyyy");
            ViewBag.RecentExpenses = expenses.Take(5).ToList();
            ViewBag.CategorySummaries = monthlySummary.CategorySummaries;

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            ViewBag.ErrorMessage = "An error occurred while loading the dashboard.";
            return View();
        }
    }
}
