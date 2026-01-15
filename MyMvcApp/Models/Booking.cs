namespace MyMvcApp.Models;

public class Booking
{
    public int BookingId { get; set; }
    public int ServiceId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public DateTime BookingDateTime { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Accepted, Completed, Cancelled
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Service Service { get; set; } = null!;
    public virtual ApplicationUser Customer { get; set; } = null!;
    public virtual ApplicationUser Provider { get; set; } = null!;
    public virtual Review? Review { get; set; }
}





