using MongoDB.Driver;
using MyMvcApp.Data;
using MyMvcApp.Models;

namespace MyMvcApp.Repositories;

public class MongoReviewRepository : IReviewRepository
{
    private readonly IMongoDbService _mongoDb;
    private readonly IMongoCollection<ReviewDocument> _collection;

    public MongoReviewRepository(IMongoDbService mongoDb)
    {
        _mongoDb = mongoDb;
        var database = _mongoDb.GetDatabase();
        _collection = database.GetCollection<ReviewDocument>("reviews");
    }

    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        var docs = await _collection.Find(_ => true).ToListAsync();
        return docs.Select(d => d.ToReview()).ToList();
    }

    public async Task<IEnumerable<Review>> GetByServiceIdAsync(int serviceId)
    {
        var docs = await _collection.Find(r => r.ServiceId == serviceId).ToListAsync();
        return docs.Select(d => d.ToReview()).ToList();
    }

    public async Task<IEnumerable<Review>> GetByProviderIdAsync(string providerId)
    {
        // Note: This requires joining with bookings/services - simplified for now
        var docs = await _collection.Find(_ => true).ToListAsync();
        return docs.Select(d => d.ToReview()).ToList();
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        var doc = await _collection.Find(r => r.ReviewId == id).FirstOrDefaultAsync();
        return doc?.ToReview();
    }

    public async Task<Review?> GetByBookingIdAsync(int bookingId)
    {
        var doc = await _collection.Find(r => r.BookingId == bookingId).FirstOrDefaultAsync();
        return doc?.ToReview();
    }

    public async Task<int> CreateAsync(Review review)
    {
        var maxId = await _collection.Find(_ => true)
            .SortByDescending(r => r.ReviewId)
            .Limit(1)
            .FirstOrDefaultAsync();
        
        var nextId = (maxId?.ReviewId ?? 0) + 1;
        
        var doc = ReviewDocument.FromReview(review, nextId);
        await _collection.InsertOneAsync(doc);
        return nextId;
    }

    public async Task<bool> UpdateAsync(Review review)
    {
        var doc = ReviewDocument.FromReview(review, review.ReviewId);
        var result = await _collection.ReplaceOneAsync(r => r.ReviewId == review.ReviewId, doc);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _collection.DeleteOneAsync(r => r.ReviewId == id);
        return result.DeletedCount > 0;
    }

    public async Task<double> GetAverageRatingByProviderIdAsync(string providerId)
    {
        // Simplified - would need to join with bookings/services
        var reviews = await _collection.Find(_ => true).ToListAsync();
        if (!reviews.Any()) return 0;
        return reviews.Average(r => r.Rating);
    }
}

public class ReviewDocument
{
    public string Id { get; set; } = string.Empty;
    public int ReviewId { get; set; }
    public int BookingId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public int ServiceId { get; set; } // Added for easier queries
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime Date { get; set; }

    public static ReviewDocument FromReview(Review review, int reviewId)
    {
        return new ReviewDocument
        {
            Id = Guid.NewGuid().ToString(),
            ReviewId = reviewId,
            BookingId = review.BookingId,
            CustomerId = review.CustomerId,
            Rating = review.Rating,
            Comment = review.Comment,
            Date = review.Date
        };
    }

    public Review ToReview()
    {
        return new Review
        {
            ReviewId = ReviewId,
            BookingId = BookingId,
            CustomerId = CustomerId,
            Rating = Rating,
            Comment = Comment,
            Date = Date
        };
    }
}
