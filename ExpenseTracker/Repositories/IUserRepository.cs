using ExpenseTracker.Models;

namespace ExpenseTracker.Repositories;

/// <summary>
/// Repository interface for User data access operations
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(string id);
}