using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MyMvcApp.Data;

namespace MyMvcApp.Controllers;

public class MongoTestController : Controller
{
    private readonly IMongoDbService _mongoDb;
    private readonly ILogger<MongoTestController> _logger;

    public MongoTestController(IMongoDbService mongoDb, ILogger<MongoTestController> logger)
    {
        _mongoDb = mongoDb;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            var database = _mongoDb.GetDatabase();
            var collection = database.GetCollection<MongoTestDocument>("testCollection");
            
            // Test insert
            var testDoc = new MongoTestDocument
            {
                Id = Guid.NewGuid().ToString(),
                Message = "MongoDB connection test",
                CreatedAt = DateTime.UtcNow
            };
            
            await collection.InsertOneAsync(testDoc);
            
            // Test read
            var count = await collection.CountDocumentsAsync(_ => true);
            var documents = await collection.Find(_ => true).Limit(10).ToListAsync();
            
            return Json(new
            {
                success = true,
                message = "MongoDB connection successful!",
                documentCount = count,
                documents = documents
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MongoDB connection test failed");
            return Json(new
            {
                success = false,
                message = $"MongoDB connection failed: {ex.Message}",
                error = ex.ToString()
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ViewMongoData()
    {
        try
        {
            var database = _mongoDb.GetDatabase();
            
            // Get test data
            var testDataCollection = database.GetCollection<MongoTestData>("testData");
            var testData = await testDataCollection.Find(_ => true).ToListAsync();
            
            // Get app info
            var appInfoCollection = database.GetCollection<AppInfo>("appInfo");
            var appInfo = await appInfoCollection.Find(_ => true).ToListAsync();
            
            // Get test collection
            var testCollection = database.GetCollection<MongoTestDocument>("testCollection");
            var testDocs = await testCollection.Find(_ => true).Limit(10).ToListAsync();
            
            return Json(new
            {
                success = true,
                message = "MongoDB data retrieved successfully",
                testData = testData,
                appInfo = appInfo,
                testDocuments = testDocs,
                summary = new
                {
                    testDataCount = testData.Count,
                    appInfoCount = appInfo.Count,
                    testDocCount = testDocs.Count
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve MongoDB data");
            return Json(new
            {
                success = false,
                message = $"Failed to retrieve data: {ex.Message}",
                error = ex.ToString()
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> StoreData([FromBody] dynamic data)
    {
        try
        {
            var database = _mongoDb.GetDatabase();
            var collection = database.GetCollection<MongoTestDocument>("testCollection");
            
            var document = new MongoTestDocument
            {
                Id = Guid.NewGuid().ToString(),
                Message = data?.ToString() ?? "Test data",
                CreatedAt = DateTime.UtcNow,
                Data = data
            };
            
            await collection.InsertOneAsync(document);
            
            return Json(new
            {
                success = true,
                message = "Data stored successfully in MongoDB",
                documentId = document.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store data in MongoDB");
            return Json(new
            {
                success = false,
                message = $"Failed to store data: {ex.Message}"
            });
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

public class MongoTestDocument
{
    public string Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public object? Data { get; set; }
}
