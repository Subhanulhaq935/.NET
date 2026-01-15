namespace MyMvcApp.Models;

public class Review
{
    public int ReviewId { get; set; }
    public int BookingId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Booking Booking { get; set; } = null!;
    public virtual ApplicationUser Customer { get; set; } = null!;
}





