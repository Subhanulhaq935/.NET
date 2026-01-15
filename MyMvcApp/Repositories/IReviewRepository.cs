using MyMvcApp.Models;

namespace MyMvcApp.Repositories;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllAsync();
    Task<IEnumerable<Review>> GetByServiceIdAsync(int serviceId);
    Task<IEnumerable<Review>> GetByProviderIdAsync(string providerId);
    Task<Review?> GetByIdAsync(int id);
    Task<Review?> GetByBookingIdAsync(int bookingId);
    Task<int> CreateAsync(Review review);
    Task<bool> UpdateAsync(Review review);
    Task<bool> DeleteAsync(int id);
    Task<double> GetAverageRatingByProviderIdAsync(string providerId);
}





