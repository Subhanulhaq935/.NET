using MongoDB.Driver;
using MyMvcApp.Data;

namespace MyMvcApp.Services;

public interface IMongoDbSeeder
{
    Task SeedAsync();
}

public class MongoDbSeeder : IMongoDbSeeder
{
    private readonly IMongoDbService _mongoDb;
    private readonly ILogger<MongoDbSeeder> _logger;

    public MongoDbSeeder(IMongoDbService mongoDb, ILogger<MongoDbSeeder> logger)
    {
        _mongoDb = mongoDb;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting MongoDB data seeding...");

            var database = _mongoDb.GetDatabase();
            
            // Create a test collection and insert sample data
            var testCollection = database.GetCollection<MongoTestData>("testData");
            
            // Check if data already exists
            var existingCount = await testCollection.CountDocumentsAsync(_ => true);
            if (existingCount == 0)
            {
                var testData = new MongoTestData
                {
                    Id = Guid.NewGuid().ToString(),
                    Message = "MongoDB is working!",
                    CreatedAt = DateTime.UtcNow,
                    ApplicationName = "MyMvcApp",
                    Status = "Active"
                };
                
                await testCollection.InsertOneAsync(testData);
                _logger.LogInformation("Inserted test data into MongoDB");
            }
            else
            {
                _logger.LogInformation("MongoDB test data already exists ({Count} documents)", existingCount);
            }

            // Create collections for application data
            var appInfoCollection = database.GetCollection<AppInfo>("appInfo");
            var appInfo = await appInfoCollection.Find(_ => true).FirstOrDefaultAsync();
            
            if (appInfo == null)
            {
                var newAppInfo = new AppInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    AppName = "MyMvcApp",
                    Version = "1.0.0",
                    MongoDBConnected = true,
                    ConnectedAt = DateTime.UtcNow,
                    ConnectionString = "mongodb://localhost:27017/"
                };
                
                await appInfoCollection.InsertOneAsync(newAppInfo);
                _logger.LogInformation("Created app info in MongoDB");
            }

            _logger.LogInformation("MongoDB data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during MongoDB data seeding.");
            throw;
        }
    }
}

public class MongoTestData
{
    public string Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string ApplicationName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class AppInfo
{
    public string Id { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool MongoDBConnected { get; set; }
    public DateTime ConnectedAt { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}
