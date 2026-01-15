using Microsoft.AspNetCore.Identity;
using MyMvcApp.Models;
using MyMvcApp.Repositories;

namespace MyMvcApp.Services;

public interface IDataSeeder
{
    Task SeedAsync();
}

public class DataSeeder : IDataSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ICategoryRepository categoryRepository,
        IServiceRepository serviceRepository,
        ILogger<DataSeeder> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _categoryRepository = categoryRepository;
        _serviceRepository = serviceRepository;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting data seeding...");

            await SeedRolesAsync().ConfigureAwait(false);
            await SeedUsersAsync().ConfigureAwait(false);
            await SeedCategoriesAsync().ConfigureAwait(false);
            await SeedSampleServicesAsync().ConfigureAwait(false);

            _logger.LogInformation("Data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during data seeding.");
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        string[] roles = { "Admin", "Provider", "Customer" };
        
        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName).ConfigureAwait(false))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName)).ConfigureAwait(false);
                _logger.LogInformation("Created role: {RoleName}", roleName);
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        // Seed Admin User
        var adminEmail = "admin@skillhub.com";
        var adminUser = await _userManager.FindByEmailAsync(adminEmail).ConfigureAwait(false);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Admin User",
                Role = "Admin",
                IsApproved = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(adminUser, "Admin@123").ConfigureAwait(false);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin").ConfigureAwait(false);
                _logger.LogInformation("Created admin user: {Email}", adminEmail);
            }
        }

        // Seed Sample Provider
        var providerEmail = "provider@skillhub.com";
        var providerUser = await _userManager.FindByEmailAsync(providerEmail).ConfigureAwait(false);
        if (providerUser == null)
        {
            providerUser = new ApplicationUser
            {
                UserName = providerEmail,
                Email = providerEmail,
                Name = "John Provider",
                Role = "Provider",
                IsApproved = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(providerUser, "Provider@123").ConfigureAwait(false);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(providerUser, "Provider").ConfigureAwait(false);
                _logger.LogInformation("Created provider user: {Email}", providerEmail);
            }
        }

        // Seed Sample Customer
        var customerEmail = "customer@skillhub.com";
        var customerUser = await _userManager.FindByEmailAsync(customerEmail).ConfigureAwait(false);
        if (customerUser == null)
        {
            customerUser = new ApplicationUser
            {
                UserName = customerEmail,
                Email = customerEmail,
                Name = "Jane Customer",
                Role = "Customer",
                IsApproved = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(customerUser, "Customer@123").ConfigureAwait(false);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(customerUser, "Customer").ConfigureAwait(false);
                _logger.LogInformation("Created customer user: {Email}", customerEmail);
            }
        }
    }

    private async Task SeedCategoriesAsync()
    {
        var categories = new[]
        {
            new Category { Name = "Plumbing", Description = "Plumbing services and repairs" },
            new Category { Name = "Tutoring", Description = "Educational tutoring services" },
            new Category { Name = "Cleaning", Description = "House and office cleaning services" },
            new Category { Name = "Electrical", Description = "Electrical repair and installation" },
            new Category { Name = "Carpentry", Description = "Woodwork and furniture services" },
            new Category { Name = "Landscaping", Description = "Garden and lawn care services" },
            new Category { Name = "Painting", Description = "Interior and exterior painting" },
            new Category { Name = "HVAC", Description = "Heating, ventilation, and air conditioning" }
        };

        var existingCategories = await _categoryRepository.GetAllAsync().ConfigureAwait(false);
        var existingNames = existingCategories.Select(c => c.Name).ToHashSet();

        foreach (var category in categories)
        {
            if (!existingNames.Contains(category.Name))
            {
                await _categoryRepository.CreateAsync(category).ConfigureAwait(false);
                _logger.LogInformation("Created category: {CategoryName}", category.Name);
            }
        }
    }

    private async Task SeedSampleServicesAsync()
    {
        var provider = await _userManager.FindByEmailAsync("provider@skillhub.com").ConfigureAwait(false);
        if (provider == null) return;

        var categories = await _categoryRepository.GetAllAsync().ConfigureAwait(false);
        var plumbingCategory = categories.FirstOrDefault(c => c.Name == "Plumbing");
        var cleaningCategory = categories.FirstOrDefault(c => c.Name == "Cleaning");
        var tutoringCategory = categories.FirstOrDefault(c => c.Name == "Tutoring");

        if (plumbingCategory == null || cleaningCategory == null || tutoringCategory == null) return;

        var existingServices = await _serviceRepository.GetByProviderIdAsync(provider.Id).ConfigureAwait(false);
        if (existingServices.Any()) return; // Don't seed if services already exist

        var services = new[]
        {
            new Service
            {
                ProviderId = provider.Id,
                CategoryId = plumbingCategory.CategoryId,
                Title = "Emergency Plumbing Repair",
                Description = "24/7 emergency plumbing services. Fix leaks, clogs, and other plumbing issues quickly and efficiently.",
                Price = 150.00m,
                Location = "Downtown",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Service
            {
                ProviderId = provider.Id,
                CategoryId = cleaningCategory.CategoryId,
                Title = "Deep House Cleaning",
                Description = "Comprehensive deep cleaning service for your home. Includes all rooms, windows, and detailed cleaning.",
                Price = 200.00m,
                Location = "Citywide",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Service
            {
                ProviderId = provider.Id,
                CategoryId = tutoringCategory.CategoryId,
                Title = "Math Tutoring - High School",
                Description = "Experienced tutor for high school mathematics. Algebra, Geometry, Calculus, and more.",
                Price = 50.00m,
                Location = "Online/In-Person",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var service in services)
        {
            await _serviceRepository.CreateAsync(service).ConfigureAwait(false);
            _logger.LogInformation("Created service: {ServiceTitle}", service.Title);
        }
    }
}





