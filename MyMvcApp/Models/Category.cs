namespace MyMvcApp.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Navigation property
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}





