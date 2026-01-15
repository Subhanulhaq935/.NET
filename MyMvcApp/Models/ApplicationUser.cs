using Microsoft.AspNetCore.Identity;

namespace MyMvcApp.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Admin, Customer, Provider
    public bool IsApproved { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    public virtual ICollection<Booking> CustomerBookings { get; set; } = new List<Booking>();
    public virtual ICollection<Booking> ProviderBookings { get; set; } = new List<Booking>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
}





