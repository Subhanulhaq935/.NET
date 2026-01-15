using ExpenseTracker.DTOs.Report;

namespace ExpenseTracker.Services;

/// <summary>
/// Service interface for reports business logic
/// </summary>
public interface IReportService
{
    Task<MonthlySummaryDto> GetMonthlySummaryAsync(string userId, int year, int month);
    Task<List<CategorySummaryDto>> GetCategorySummaryAsync(string userId);
    Task<List<CategorySummaryDto>> GetCategorySummaryByMonthAsync(string userId, int year, int month);
}