using MyMvcApp.Models;

namespace MyMvcApp.Repositories;

public interface IAvailabilityRepository
{
    Task<IEnumerable<Availability>> GetByProviderIdAsync(string providerId);
    Task<IEnumerable<Availability>> GetAvailableSlotsAsync(string providerId, DateTime? fromDate = null);
    Task<Availability?> GetByIdAsync(int id);
    Task<int> CreateAsync(Availability availability);
    Task<bool> UpdateAsync(Availability availability);
    Task<bool> DeleteAsync(int id);
}





