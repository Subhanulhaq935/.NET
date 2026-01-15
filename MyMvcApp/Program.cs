using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MyMvcApp.Data;
using MyMvcApp.Models;
using MyMvcApp.Repositories;
using MyMvcApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(
    "https://localhost:7156",
    "http://localhost:5165"
);

// MongoDB Service (must be registered first)
builder.Services.AddSingleton<IMongoDbService, MongoDbService>();

// Identity Framework with MongoDB stores
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    
    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false;
})
.AddUserStore<MongoUserStore>()
.AddRoleStore<MongoRoleStore>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Repository Pattern - Dependency Injection (MongoDB)
builder.Services.AddScoped<ICategoryRepository, MongoCategoryRepository>();
builder.Services.AddScoped<IServiceRepository, MongoServiceRepository>();
builder.Services.AddScoped<IBookingRepository, MongoBookingRepository>();
builder.Services.AddScoped<IReviewRepository, MongoReviewRepository>();
builder.Services.AddScoped<IAvailabilityRepository, MongoAvailabilityRepository>();

// Cookie Service for State Management
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICookieService, CookieService>();

// Data Seeding Service
builder.Services.AddScoped<IDataSeeder, DataSeeder>();

// MongoDB Seeding Service
builder.Services.AddScoped<IMongoDbSeeder, MongoDbSeeder>();

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Provider must be approved
    options.AddPolicy("ProviderApproved", policy =>
        policy.Requirements.Add(new MyMvcApp.Authorization.Requirements.ProviderApprovedRequirement()));

    // Resource owner policy
    options.AddPolicy("ResourceOwner", policy =>
        policy.Requirements.Add(new MyMvcApp.Authorization.Requirements.ResourceOwnerRequirement()));

    // Booking access policy
    options.AddPolicy("BookingAccess", policy =>
        policy.Requirements.Add(new MyMvcApp.Authorization.Requirements.BookingAccessRequirement()));
});

// Register Authorization Handlers
builder.Services.AddScoped<IAuthorizationHandler, MyMvcApp.Authorization.Handlers.ProviderApprovedHandler>();
builder.Services.AddScoped<IAuthorizationHandler, MyMvcApp.Authorization.Handlers.ResourceOwnerHandler>();
builder.Services.AddScoped<IAuthorizationHandler, MyMvcApp.Authorization.Handlers.BookingAccessHandler>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed data using DataSeeder service
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Seed MongoDB data (Identity and application data)
        var seeder = services.GetRequiredService<IDataSeeder>();
        await seeder.SeedAsync().ConfigureAwait(false);
        
        // Seed MongoDB test data
        var mongoSeeder = services.GetRequiredService<IMongoDbSeeder>();
        await mongoSeeder.SeedAsync().ConfigureAwait(false);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
