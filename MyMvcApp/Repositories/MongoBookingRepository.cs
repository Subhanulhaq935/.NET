using MongoDB.Driver;
using MyMvcApp.Data;
using MyMvcApp.Models;

namespace MyMvcApp.Repositories;

public class MongoBookingRepository : IBookingRepository
{
    private readonly IMongoDbService _mongoDb;
    private readonly IMongoCollection<BookingDocument> _collection;

    public MongoBookingRepository(IMongoDbService mongoDb)
    {
        _mongoDb = mongoDb;
        var database = _mongoDb.GetDatabase();
        _collection = database.GetCollection<BookingDocument>("bookings");
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        var docs = await _collection.Find(_ => true).SortByDescending(b => b.CreatedAt).ToListAsync();
        return docs.Select(d => d.ToBooking()).ToList();
    }

    public async Task<IEnumerable<Booking>> GetByCustomerIdAsync(string customerId)
    {
        var docs = await _collection.Find(b => b.CustomerId == customerId).ToListAsync();
        return docs.Select(d => d.ToBooking()).ToList();
    }

    public async Task<IEnumerable<Booking>> GetByProviderIdAsync(string providerId)
    {
        var docs = await _collection.Find(b => b.ProviderId == providerId).ToListAsync();
        return docs.Select(d => d.ToBooking()).ToList();
    }

    public async Task<IEnumerable<Booking>> GetByServiceIdAsync(int serviceId)
    {
        var docs = await _collection.Find(b => b.ServiceId == serviceId).ToListAsync();
        return docs.Select(d => d.ToBooking()).ToList();
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        var doc = await _collection.Find(b => b.BookingId == id).FirstOrDefaultAsync();
        return doc?.ToBooking();
    }

    public async Task<Booking?> GetByIdWithDetailsAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<int> CreateAsync(Booking booking)
    {
        var maxId = await _collection.Find(_ => true)
            .SortByDescending(b => b.BookingId)
            .Limit(1)
            .FirstOrDefaultAsync();
        
        var nextId = (maxId?.BookingId ?? 0) + 1;
        
        var doc = BookingDocument.FromBooking(booking, nextId);
        await _collection.InsertOneAsync(doc);
        return nextId;
    }

    public async Task<bool> UpdateAsync(Booking booking)
    {
        var doc = BookingDocument.FromBooking(booking, booking.BookingId);
        var result = await _collection.ReplaceOneAsync(b => b.BookingId == booking.BookingId, doc);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateStatusAsync(int bookingId, string status)
    {
        var result = await _collection.UpdateOneAsync(
            b => b.BookingId == bookingId,
            Builders<BookingDocument>.Update.Set(b => b.Status, status));
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _collection.DeleteOneAsync(b => b.BookingId == id);
        return result.DeletedCount > 0;
    }
}

public class BookingDocument
{
    public string Id { get; set; } = string.Empty;
    public int BookingId { get; set; }
    public int ServiceId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public DateTime BookingDateTime { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public static BookingDocument FromBooking(Booking booking, int bookingId)
    {
        return new BookingDocument
        {
            Id = Guid.NewGuid().ToString(),
            BookingId = bookingId,
            ServiceId = booking.ServiceId,
            CustomerId = booking.CustomerId,
            ProviderId = booking.ProviderId,
            BookingDateTime = booking.BookingDateTime,
            Status = booking.Status,
            Notes = booking.Notes,
            CreatedAt = booking.CreatedAt
        };
    }

    public Booking ToBooking()
    {
        return new Booking
        {
            BookingId = BookingId,
            ServiceId = ServiceId,
            CustomerId = CustomerId,
            ProviderId = ProviderId,
            BookingDateTime = BookingDateTime,
            Status = Status,
            Notes = Notes,
            CreatedAt = CreatedAt
        };
    }
}
