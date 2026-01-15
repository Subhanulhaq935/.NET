using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.DTOs.Expense;
using ExpenseTracker.Services;

namespace ExpenseTracker.Controllers;

/// <summary>
/// Controller for expense management operations
/// All endpoints require authentication
/// </summary>
[Authorize]
public class ExpensesController : Controller
{
    private readonly IExpenseService _expenseService;
    private readonly ILogger<ExpensesController> _logger;

    public ExpensesController(IExpenseService expenseService, ILogger<ExpensesController> logger)
    {
        _expenseService = expenseService;
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
    /// Displays the expense list with filtering options
    /// GET /Expenses/Index
    /// </summary>
    public async Task<IActionResult> Index(
        string? category = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? year = null,
        int? month = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            List<ExpenseResponseDto> expenses;

            // Apply filters based on query parameters
            if (year.HasValue && month.HasValue)
            {
                expenses = await _expenseService.GetExpensesByMonthAsync(userId, year.Value, month.Value);
            }
            else if (startDate.HasValue && endDate.HasValue)
            {
                expenses = await _expenseService.GetExpensesByDateRangeAsync(userId, startDate.Value, endDate.Value);
            }
            else if (!string.IsNullOrEmpty(category))
            {
                expenses = await _expenseService.GetExpensesByCategoryAsync(userId, category);
            }
            else
            {
                expenses = await _expenseService.GetUserExpensesAsync(userId);
            }

            // Get all unique categories for filter dropdown
            var allExpenses = await _expenseService.GetUserExpensesAsync(userId);
            var categories = allExpenses.Select(e => e.Category).Distinct().OrderBy(c => c).ToList();

            var viewModel = new ExpenseListViewModel
            {
                Expenses = expenses,
                TotalAmount = expenses.Sum(e => e.Amount),
                TotalCount = expenses.Count,
                FilterCategory = category,
                FilterStartDate = startDate,
                FilterEndDate = endDate,
                FilterYear = year,
                FilterMonth = month,
                Categories = categories
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading expenses");
            ViewBag.ErrorMessage = "An error occurred while loading expenses.";
            return View(new ExpenseListViewModel());
        }
    }

    /// <summary>
    /// Displays the create expense form
    /// GET /Expenses/Create
    /// </summary>
    public IActionResult Create()
    {
        return View(new ExpenseViewModel { Date = DateTime.Today });
    }

    /// <summary>
    /// Processes expense creation
    /// POST /Expenses/Create
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userId = GetCurrentUserId();
            var createDto = new CreateExpenseDto
            {
                Title = model.Title,
                Amount = model.Amount,
                Category = model.Category,
                Date = model.Date,
                Notes = model.Notes
            };

            await _expenseService.CreateExpenseAsync(userId, createDto);
            TempData["SuccessMessage"] = "Expense created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating expense");
            ModelState.AddModelError("", "An error occurred while creating the expense.");
            return View(model);
        }
    }

    /// <summary>
    /// Displays the edit expense form
    /// GET /Expenses/Edit/{id}
    /// </summary>
    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var expense = await _expenseService.GetExpenseByIdAsync(userId, id);

            if (expense == null)
            {
                TempData["ErrorMessage"] = "Expense not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ExpenseViewModel
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                Category = expense.Category,
                Date = expense.Date,
                Notes = expense.Notes
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading expense for edit");
            TempData["ErrorMessage"] = "An error occurred while loading the expense.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Processes expense update
    /// POST /Expenses/Edit/{id}
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, ExpenseViewModel model)
    {
        if (id != model.Id)
        {
            TempData["ErrorMessage"] = "Invalid expense ID.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userId = GetCurrentUserId();
            var updateDto = new UpdateExpenseDto
            {
                Title = model.Title,
                Amount = model.Amount,
                Category = model.Category,
                Date = model.Date,
                Notes = model.Notes
            };

            var result = await _expenseService.UpdateExpenseAsync(userId, id, updateDto);

            if (result == null)
            {
                TempData["ErrorMessage"] = "Expense not found.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Expense updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating expense");
            ModelState.AddModelError("", "An error occurred while updating the expense.");
            return View(model);
        }
    }

    /// <summary>
    /// Displays the delete confirmation page
    /// GET /Expenses/Delete/{id}
    /// </summary>
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var expense = await _expenseService.GetExpenseByIdAsync(userId, id);

            if (expense == null)
            {
                TempData["ErrorMessage"] = "Expense not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(expense);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading expense for delete");
            TempData["ErrorMessage"] = "An error occurred while loading the expense.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Processes expense deletion
    /// POST /Expenses/Delete/{id}
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var deleted = await _expenseService.DeleteExpenseAsync(userId, id);

            if (!deleted)
            {
                TempData["ErrorMessage"] = "Expense not found.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Expense deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting expense");
            TempData["ErrorMessage"] = "An error occurred while deleting the expense.";
            return RedirectToAction(nameof(Index));
        }
    }
}
