using MongoDB.Driver;
using ExpenseTracker.Models;

namespace ExpenseTracker.Repositories;

/// <summary>
/// Repository implementation for Expense data access using MongoDB
/// </summary>
public class ExpenseRepository : IExpenseRepository
{
    private readonly IMongoCollection<Expense> _expenses;

    public ExpenseRepository(IMongoDatabase database)
    {
        _expenses = database.GetCollection<Expense>("Expenses");
        
        // Create index on userId for faster queries
        var indexDefinition = Builders<Expense>.IndexKeys.Ascending(e => e.UserId);
        _expenses.Indexes.CreateOne(new CreateIndexModel<Expense>(indexDefinition));
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        await _expenses.InsertOneAsync(expense);
        return expense;
    }

    public async Task<Expense?> GetByIdAsync(string id, string userId)
    {
        return await _expenses.Find(e => e.Id == id && e.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<List<Expense>> GetByUserIdAsync(string userId)
    {
        return await _expenses.Find(e => e.UserId == userId)
            .SortByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<Expense>> GetByUserIdAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<Expense>.Filter.And(
            Builders<Expense>.Filter.Eq(e => e.UserId, userId),
            Builders<Expense>.Filter.Gte(e => e.Date, startDate.Date),
            Builders<Expense>.Filter.Lte(e => e.Date, endDate.Date.AddDays(1).AddTicks(-1))
        );
        
        return await _expenses.Find(filter)
            .SortByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<Expense>> GetByUserIdAndCategoryAsync(string userId, string category)
    {
        return await _expenses.Find(e => e.UserId == userId && e.Category == category)
            .SortByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<Expense>> GetByUserIdAndMonthAsync(string userId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddTicks(-1);
        
        return await GetByUserIdAndDateRangeAsync(userId, startDate, endDate);
    }

    public async Task<Expense?> UpdateAsync(string id, string userId, Expense expense)
    {
        var filter = Builders<Expense>.Filter.And(
            Builders<Expense>.Filter.Eq(e => e.Id, id),
            Builders<Expense>.Filter.Eq(e => e.UserId, userId)
        );

        var update = Builders<Expense>.Update
            .Set(e => e.Title, expense.Title)
            .Set(e => e.Amount, expense.Amount)
            .Set(e => e.Category, expense.Category)
            .Set(e => e.Date, expense.Date)
            .Set(e => e.Notes, expense.Notes)
            .Set(e => e.UpdatedAt, DateTime.UtcNow);

        var options = new FindOneAndUpdateOptions<Expense>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await _expenses.FindOneAndUpdateAsync(filter, update, options);
    }

    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var filter = Builders<Expense>.Filter.And(
            Builders<Expense>.Filter.Eq(e => e.Id, id),
            Builders<Expense>.Filter.Eq(e => e.UserId, userId)
        );

        var result = await _expenses.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
}