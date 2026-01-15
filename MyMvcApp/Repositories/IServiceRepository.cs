using MyMvcApp.Models;

namespace MyMvcApp.Repositories;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetAllAsync();
    Task<IEnumerable<Service>> GetByProviderIdAsync(string providerId);
    Task<IEnumerable<Service>> GetActiveServicesAsync();
    Task<IEnumerable<Service>> SearchAsync(string? categoryId, string? searchTerm);
    Task<Service?> GetByIdAsync(int id);
    Task<Service?> GetByIdWithDetailsAsync(int id);
    Task<int> CreateAsync(Service service);
    Task<bool> UpdateAsync(Service service);
    Task<bool> DeleteAsync(int id);
}





