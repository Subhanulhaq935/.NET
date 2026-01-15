namespace ExpenseTracker.DTOs.Report;

/// <summary>
/// Data Transfer Object for monthly expense summary
/// </summary>
public class MonthlySummaryDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalAmount { get; set; }
    public int TotalExpenses { get; set; }
    public List<CategorySummaryDto> CategorySummaries { get; set; } = new();
}