using MongoDB.Driver;
using MyMvcApp.Data;
using MyMvcApp.Models;

namespace MyMvcApp.Repositories;

public class MongoCategoryRepository : ICategoryRepository
{
    private readonly IMongoDbService _mongoDb;
    private readonly IMongoCollection<CategoryDocument> _collection;

    public MongoCategoryRepository(IMongoDbService mongoDb)
    {
        _mongoDb = mongoDb;
        var database = _mongoDb.GetDatabase();
        _collection = database.GetCollection<CategoryDocument>("categories");
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        var documents = await _collection.Find(_ => true).SortBy(c => c.Name).ToListAsync();
        return documents.Select(d => d.ToCategory()).ToList();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        var document = await _collection.Find(c => c.CategoryId == id).FirstOrDefaultAsync();
        return document?.ToCategory();
    }

    public async Task<int> CreateAsync(Category category)
    {
        // Get next ID
        var maxId = await _collection.Find(_ => true)
            .SortByDescending(c => c.CategoryId)
            .Limit(1)
            .FirstOrDefaultAsync();
        
        var nextId = (maxId?.CategoryId ?? 0) + 1;
        
        var document = new CategoryDocument
        {
            Id = Guid.NewGuid().ToString(),
            CategoryId = nextId,
            Name = category.Name,
            Description = category.Description
        };
        
        await _collection.InsertOneAsync(document);
        return nextId;
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        var result = await _collection.ReplaceOneAsync(
            c => c.CategoryId == category.CategoryId,
            new CategoryDocument
            {
                Id = Guid.NewGuid().ToString(), // In real app, you'd fetch existing doc first
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            });
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _collection.DeleteOneAsync(c => c.CategoryId == id);
        return result.DeletedCount > 0;
    }
}

public class CategoryDocument
{
    public string Id { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Category ToCategory()
    {
        return new Category
        {
            CategoryId = CategoryId,
            Name = Name,
            Description = Description
        };
    }
}
