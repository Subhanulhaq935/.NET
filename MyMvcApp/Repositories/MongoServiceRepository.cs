using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MyMvcApp.Data;
using MyMvcApp.Models;

namespace MyMvcApp.Repositories;

public class MongoServiceRepository : IServiceRepository
{
    private readonly IMongoDbService _mongoDb;
    private readonly IMongoCollection<ServiceDocument> _collection;
    private readonly ICategoryRepository _categoryRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public MongoServiceRepository(
        IMongoDbService mongoDb,
        ICategoryRepository categoryRepository,
        UserManager<ApplicationUser> userManager)
    {
        _mongoDb = mongoDb;
        var database = _mongoDb.GetDatabase();
        _collection = database.GetCollection<ServiceDocument>("services");
        _categoryRepository = categoryRepository;
        _userManager = userManager;
    }

    private async Task<Service> PopulateNavigationPropertiesAsync(ServiceDocument doc)
    {
        var service = doc.ToService();
        
        // Populate Category
        if (doc.CategoryId > 0)
        {
            service.Category = await _categoryRepository.GetByIdAsync(doc.CategoryId);
        }
        
        // Populate Provider
        if (!string.IsNullOrEmpty(doc.ProviderId))
        {
            service.Provider = await _userManager.FindByIdAsync(doc.ProviderId);
        }
        
        return service;
    }

    public async Task<IEnumerable<Service>> GetAllAsync()
    {
        var docs = await _collection.Find(_ => true).ToListAsync();
        var services = new List<Service>();
        foreach (var doc in docs)
        {
            services.Add(await PopulateNavigationPropertiesAsync(doc));
        }
        return services;
    }

    public async Task<IEnumerable<Service>> GetByProviderIdAsync(string providerId)
    {
        var docs = await _collection.Find(s => s.ProviderId == providerId).ToListAsync();
        var services = new List<Service>();
        foreach (var doc in docs)
        {
            services.Add(await PopulateNavigationPropertiesAsync(doc));
        }
        return services;
    }

    public async Task<IEnumerable<Service>> GetActiveServicesAsync()
    {
        var docs = await _collection.Find(s => s.IsActive).ToListAsync();
        var services = new List<Service>();
        foreach (var doc in docs)
        {
            services.Add(await PopulateNavigationPropertiesAsync(doc));
        }
        return services;
    }

    public async Task<IEnumerable<Service>> SearchAsync(string? categoryId, string? searchTerm)
    {
        var filter = Builders<ServiceDocument>.Filter.Empty;
        
        if (!string.IsNullOrEmpty(categoryId) && int.TryParse(categoryId, out var catId))
        {
            filter &= Builders<ServiceDocument>.Filter.Eq(s => s.CategoryId, catId);
        }
        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            filter &= Builders<ServiceDocument>.Filter.Or(
                Builders<ServiceDocument>.Filter.Regex(s => s.Title, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<ServiceDocument>.Filter.Regex(s => s.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }
        
        var docs = await _collection.Find(filter).ToListAsync();
        var services = new List<Service>();
        foreach (var doc in docs)
        {
            services.Add(await PopulateNavigationPropertiesAsync(doc));
        }
        return services;
    }

    public async Task<Service?> GetByIdAsync(int id)
    {
        var doc = await _collection.Find(s => s.ServiceId == id).FirstOrDefaultAsync();
        if (doc == null) return null;
        return await PopulateNavigationPropertiesAsync(doc);
    }

    public async Task<Service?> GetByIdWithDetailsAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<int> CreateAsync(Service service)
    {
        var maxId = await _collection.Find(_ => true)
            .SortByDescending(s => s.ServiceId)
            .Limit(1)
            .FirstOrDefaultAsync();
        
        var nextId = (maxId?.ServiceId ?? 0) + 1;
        
        var doc = ServiceDocument.FromService(service, nextId);
        await _collection.InsertOneAsync(doc);
        return nextId;
    }

    public async Task<bool> UpdateAsync(Service service)
    {
        var doc = ServiceDocument.FromService(service, service.ServiceId);
        var result = await _collection.ReplaceOneAsync(s => s.ServiceId == service.ServiceId, doc);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _collection.DeleteOneAsync(s => s.ServiceId == id);
        return result.DeletedCount > 0;
    }
}

public class ServiceDocument
{
    public string Id { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public string ProviderId { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public static ServiceDocument FromService(Service service, int serviceId)
    {
        return new ServiceDocument
        {
            Id = Guid.NewGuid().ToString(),
            ServiceId = serviceId,
            ProviderId = service.ProviderId,
            CategoryId = service.CategoryId,
            Title = service.Title,
            Description = service.Description,
            Price = service.Price,
            Location = service.Location,
            IsActive = service.IsActive,
            CreatedAt = service.CreatedAt
        };
    }

    public Service ToService()
    {
        return new Service
        {
            ServiceId = ServiceId,
            ProviderId = ProviderId,
            CategoryId = CategoryId,
            Title = Title,
            Description = Description,
            Price = Price,
            Location = Location,
            IsActive = IsActive,
            CreatedAt = CreatedAt
        };
    }
}
