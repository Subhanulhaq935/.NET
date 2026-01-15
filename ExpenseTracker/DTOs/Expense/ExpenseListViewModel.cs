using ExpenseTracker.DTOs.Expense;

namespace ExpenseTracker.DTOs.Expense;

/// <summary>
/// ViewModel for expense list page
/// </summary>
public class ExpenseListViewModel
{
    public List<ExpenseResponseDto> Expenses { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public int TotalCount { get; set; }
    public string? FilterCategory { get; set; }
    public DateTime? FilterStartDate { get; set; }
    public DateTime? FilterEndDate { get; set; }
    public int? FilterYear { get; set; }
    public int? FilterMonth { get; set; }
    public List<string> Categories { get; set; } = new();
}
