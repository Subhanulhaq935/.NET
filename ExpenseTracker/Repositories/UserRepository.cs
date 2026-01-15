using MongoDB.Driver;
using ExpenseTracker.Models;
using ExpenseTracker.Settings;

namespace ExpenseTracker.Repositories;

/// <summary>
/// Repository implementation for User data access using MongoDB
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("Users");
        
        // Create unique index on email
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexDefinition = Builders<User>.IndexKeys.Ascending(u => u.Email);
        _users.Indexes.CreateOne(new CreateIndexModel<User>(indexDefinition, indexOptions));
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }
}