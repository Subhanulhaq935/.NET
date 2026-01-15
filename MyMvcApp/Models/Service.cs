using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Models;

public class Service
{
    public int ServiceId { get; set; }
    public string ProviderId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Category is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    [Display(Name = "Service Title")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between $0.01 and $999,999.99")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }
    
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    [Display(Name = "Location")]
    public string? Location { get; set; }
    
    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties (not required for form binding)
    public virtual ApplicationUser? Provider { get; set; }
    public virtual Category? Category { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

