using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.DTOs.Expense;

/// <summary>
/// ViewModel for expense form submission
/// </summary>
public class ExpenseViewModel
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [StringLength(100, ErrorMessage = "Category must not exceed 100 characters")]
    [Display(Name = "Category")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date is required")]
    [Display(Name = "Date")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
}
