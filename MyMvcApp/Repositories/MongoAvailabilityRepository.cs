using MongoDB.Driver;
using MyMvcApp.Data;
using MyMvcApp.Models;

namespace MyMvcApp.Repositories;

public class MongoAvailabilityRepository : IAvailabilityRepository
{
    private readonly IMongoDbService _mongoDb;
    private readonly IMongoCollection<AvailabilityDocument> _collection;

    public MongoAvailabilityRepository(IMongoDbService mongoDb)
    {
        _mongoDb = mongoDb;
        var database = _mongoDb.GetDatabase();
        _collection = database.GetCollection<AvailabilityDocument>("availabilities");
    }

    public async Task<IEnumerable<Availability>> GetByProviderIdAsync(string providerId)
    {
        var docs = await _collection.Find(a => a.ProviderId == providerId).ToListAsync();
        return docs.Select(d => d.ToAvailability()).ToList();
    }

    public async Task<IEnumerable<Availability>> GetAvailableSlotsAsync(string providerId, DateTime? fromDate = null)
    {
        var filter = Builders<AvailabilityDocument>.Filter.Eq(a => a.ProviderId, providerId) &
                     Builders<AvailabilityDocument>.Filter.Eq(a => a.IsAvailable, true);
        
        if (fromDate.HasValue)
        {
            filter &= Builders<AvailabilityDocument>.Filter.Gte(a => a.Date, fromDate.Value.Date);
        }
        
        var docs = await _collection.Find(filter).ToListAsync();
        return docs.Select(d => d.ToAvailability()).ToList();
    }

    public async Task<Availability?> GetByIdAsync(int id)
    {
        var doc = await _collection.Find(a => a.AvailabilityId == id).FirstOrDefaultAsync();
        return doc?.ToAvailability();
    }

    public async Task<int> CreateAsync(Availability availability)
    {
        var maxId = await _collection.Find(_ => true)
            .SortByDescending(a => a.AvailabilityId)
            .Limit(1)
            .FirstOrDefaultAsync();
        
        var nextId = (maxId?.AvailabilityId ?? 0) + 1;
        
        var doc = AvailabilityDocument.FromAvailability(availability, nextId);
        await _collection.InsertOneAsync(doc);
        return nextId;
    }

    public async Task<bool> UpdateAsync(Availability availability)
    {
        var doc = AvailabilityDocument.FromAvailability(availability, availability.AvailabilityId);
        var result = await _collection.ReplaceOneAsync(a => a.AvailabilityId == availability.AvailabilityId, doc);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _collection.DeleteOneAsync(a => a.AvailabilityId == id);
        return result.DeletedCount > 0;
    }
}

public class AvailabilityDocument
{
    public string Id { get; set; } = string.Empty;
    public int AvailabilityId { get; set; }
    public string ProviderId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;

    public static AvailabilityDocument FromAvailability(Availability availability, int availabilityId)
    {
        return new AvailabilityDocument
        {
            Id = Guid.NewGuid().ToString(),
            AvailabilityId = availabilityId,
            ProviderId = availability.ProviderId,
            Date = availability.Date,
            StartTime = availability.StartTime,
            EndTime = availability.EndTime,
            IsAvailable = availability.IsAvailable
        };
    }

    public Availability ToAvailability()
    {
        return new Availability
        {
            AvailabilityId = AvailabilityId,
            ProviderId = ProviderId,
            Date = Date,
            StartTime = StartTime,
            EndTime = EndTime,
            IsAvailable = IsAvailable
        };
    }
}
