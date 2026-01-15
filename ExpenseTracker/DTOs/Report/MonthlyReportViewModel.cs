using ExpenseTracker.DTOs.Report;

namespace ExpenseTracker.DTOs.Report;

/// <summary>
/// ViewModel for monthly report page
/// </summary>
public class MonthlyReportViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public MonthlySummaryDto Summary { get; set; } = new();
    public List<int> AvailableYears { get; set; } = new();
}
