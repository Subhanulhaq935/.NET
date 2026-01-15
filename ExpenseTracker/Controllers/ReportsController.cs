using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.DTOs.Report;
using ExpenseTracker.Services;

namespace ExpenseTracker.Controllers;

/// <summary>
/// Controller for report operations
/// All endpoints require authentication
/// </summary>
[Authorize]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
    {
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
    /// Displays monthly expense report
    /// GET /Reports/Monthly
    /// </summary>
    public async Task<IActionResult> Monthly(int? year = null, int? month = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var currentDate = DateTime.Now;

            var reportYear = year ?? currentDate.Year;
            var reportMonth = month ?? currentDate.Month;

            // Validate month and year
            if (reportMonth < 1 || reportMonth > 12)
            {
                reportMonth = currentDate.Month;
            }

            if (reportYear < 2000 || reportYear > 2100)
            {
                reportYear = currentDate.Year;
            }

            var summary = await _reportService.GetMonthlySummaryAsync(userId, reportYear, reportMonth);

            // Generate available years (last 5 years and next year)
            var availableYears = Enumerable.Range(currentDate.Year - 5, 7).ToList();

            var viewModel = new MonthlyReportViewModel
            {
                Year = reportYear,
                Month = reportMonth,
                Summary = summary,
                AvailableYears = availableYears
            };

            return View(viewModel);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid parameters for monthly report");
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Monthly");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating monthly report");
            TempData["ErrorMessage"] = "An error occurred while generating the report.";
            return RedirectToAction("Monthly");
        }
    }

    /// <summary>
    /// Displays category-wise expense summary
    /// GET /Reports/Category
    /// </summary>
    public async Task<IActionResult> Category(int? year = null, int? month = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            List<CategorySummaryDto> categorySummaries;

            if (year.HasValue && month.HasValue)
            {
                categorySummaries = await _reportService.GetCategorySummaryByMonthAsync(userId, year.Value, month.Value);
                ViewBag.Year = year.Value;
                ViewBag.Month = month.Value;
                ViewBag.MonthName = new DateTime(year.Value, month.Value, 1).ToString("MMMM yyyy");
            }
            else
            {
                categorySummaries = await _reportService.GetCategorySummaryAsync(userId);
                ViewBag.Year = null;
                ViewBag.Month = null;
                ViewBag.MonthName = "All Time";
            }

            var currentDate = DateTime.Now;
            var availableYears = Enumerable.Range(currentDate.Year - 5, 7).ToList();
            ViewBag.AvailableYears = availableYears;

            return View(categorySummaries);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid parameters for category report");
            TempData["ErrorMessage"] = ex.Message;
            return View(new List<CategorySummaryDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating category report");
            TempData["ErrorMessage"] = "An error occurred while generating the report.";
            return View(new List<CategorySummaryDto>());
        }
    }
}
