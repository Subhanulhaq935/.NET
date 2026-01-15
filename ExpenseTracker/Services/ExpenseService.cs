using ExpenseTracker.DTOs.Expense;
using ExpenseTracker.Models;
using ExpenseTracker.Repositories;

namespace ExpenseTracker.Services;

/// <summary>
/// Service implementation for expense management business logic
/// </summary>
public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseService(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    /// <summary>
    /// Creates a new expense with business logic validation
    /// Business Rules:
    /// - Amount must be positive (enforced by DTO Range attribute)
    /// - User can only create expenses for themselves (enforced by userId parameter)
    /// </summary>
    public async Task<ExpenseResponseDto> CreateExpenseAsync(string userId, CreateExpenseDto createDto)
    {
        // Business logic validation (Amount is already validated by DTO, but double-check)
        if (createDto.Amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero");
        }

        // Create expense entity from DTO
        var expense = new Expense
        {
            UserId = userId,
            Title = createDto.Title,
            Amount = createDto.Amount,
            Category = createDto.Category,
            Date = createDto.Date,
            Notes = createDto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        // Persist expense
        var createdExpense = await _expenseRepository.CreateAsync(expense);

        // Map entity to response DTO
        return MapToResponseDto(createdExpense);
    }

    /// <summary>
    /// Gets an expense by ID with authorization check
    /// Business Rules:
    /// - User can only access their own expenses (enforced by repository filter)
    /// </summary>
    public async Task<ExpenseResponseDto?> GetExpenseByIdAsync(string userId, string expenseId)
    {
        var expense = await _expenseRepository.GetByIdAsync(expenseId, userId);
        return expense == null ? null : MapToResponseDto(expense);
    }

    /// <summary>
    /// Gets all expenses for a user
    /// Business Rules:
    /// - User can only see their own expenses (enforced by repository filter)
    /// </summary>
    public async Task<List<ExpenseResponseDto>> GetUserExpensesAsync(string userId)
    {
        var expenses = await _expenseRepository.GetByUserIdAsync(userId);
        return expenses.Select(MapToResponseDto).ToList();
    }

    /// <summary>
    /// Gets expenses within a date range
    /// Business Rules:
    /// - User can only see their own expenses
    /// </summary>
    public async Task<List<ExpenseResponseDto>> GetExpensesByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
    {
        var expenses = await _expenseRepository.GetByUserIdAndDateRangeAsync(userId, startDate, endDate);
        return expenses.Select(MapToResponseDto).ToList();
    }

    /// <summary>
    /// Gets expenses by category
    /// Business Rules:
    /// - User can only see their own expenses
    /// </summary>
    public async Task<List<ExpenseResponseDto>> GetExpensesByCategoryAsync(string userId, string category)
    {
        var expenses = await _expenseRepository.GetByUserIdAndCategoryAsync(userId, category);
        return expenses.Select(MapToResponseDto).ToList();
    }

    /// <summary>
    /// Gets expenses for a specific month
    /// Business Rules:
    /// - User can only see their own expenses
    /// </summary>
    public async Task<List<ExpenseResponseDto>> GetExpensesByMonthAsync(string userId, int year, int month)
    {
        // Business logic validation
        if (month < 1 || month > 12)
        {
            throw new ArgumentException("Month must be between 1 and 12");
        }

        if (year < 1900 || year > 2100)
        {
            throw new ArgumentException("Year must be between 1900 and 2100");
        }

        var expenses = await _expenseRepository.GetByUserIdAndMonthAsync(userId, year, month);
        return expenses.Select(MapToResponseDto).ToList();
    }

    /// <summary>
    /// Updates an expense with business logic validation
    /// Business Rules:
    /// - Amount must be positive
    /// - User can only update their own expenses
    /// </summary>
    public async Task<ExpenseResponseDto?> UpdateExpenseAsync(string userId, string expenseId, UpdateExpenseDto updateDto)
    {
        // Business logic validation
        if (updateDto.Amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero");
        }

        // Get existing expense to ensure it exists and belongs to user
        var existingExpense = await _expenseRepository.GetByIdAsync(expenseId, userId);
        if (existingExpense == null)
        {
            return null;
        }

        // Update expense entity
        var expense = new Expense
        {
            Id = expenseId,
            UserId = userId,
            Title = updateDto.Title,
            Amount = updateDto.Amount,
            Category = updateDto.Category,
            Date = updateDto.Date,
            Notes = updateDto.Notes,
            CreatedAt = existingExpense.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var updatedExpense = await _expenseRepository.UpdateAsync(expenseId, userId, expense);
        return updatedExpense == null ? null : MapToResponseDto(updatedExpense);
    }

    /// <summary>
    /// Deletes an expense with authorization check
    /// Business Rules:
    /// - User can only delete their own expenses
    /// </summary>
    public async Task<bool> DeleteExpenseAsync(string userId, string expenseId)
    {
        return await _expenseRepository.DeleteAsync(expenseId, userId);
    }

    /// <summary>
    /// Maps Expense entity to ExpenseResponseDto
    /// This ensures database models are not exposed directly through the API
    /// </summary>
    private static ExpenseResponseDto MapToResponseDto(Expense expense)
    {
        return new ExpenseResponseDto
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            Category = expense.Category,
            Date = expense.Date,
            Notes = expense.Notes,
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };
    }
}