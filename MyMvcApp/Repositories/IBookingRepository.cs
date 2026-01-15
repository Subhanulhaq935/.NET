using MyMvcApp.Models;

namespace MyMvcApp.Repositories;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<IEnumerable<Booking>> GetByCustomerIdAsync(string customerId);
    Task<IEnumerable<Booking>> GetByProviderIdAsync(string providerId);
    Task<IEnumerable<Booking>> GetByServiceIdAsync(int serviceId);
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking?> GetByIdWithDetailsAsync(int id);
    Task<int> CreateAsync(Booking booking);
    Task<bool> UpdateAsync(Booking booking);
    Task<bool> UpdateStatusAsync(int bookingId, string status);
    Task<bool> DeleteAsync(int id);
}


