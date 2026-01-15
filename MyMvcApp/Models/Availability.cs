namespace MyMvcApp.Models;

public class Availability
{
    public int AvailabilityId { get; set; }
    public string ProviderId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    
    // Navigation property
    public virtual ApplicationUser Provider { get; set; } = null!;
}





