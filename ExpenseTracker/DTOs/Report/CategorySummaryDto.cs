namespace ExpenseTracker.DTOs.Report;

/// <summary>
/// Data Transfer Object for category-wise expense summary
/// </summary>
public class CategorySummaryDto
{
    public string Category { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}