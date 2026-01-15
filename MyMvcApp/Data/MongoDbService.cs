using MongoDB.Driver;

namespace MyMvcApp.Data;

public class MongoDbService : IMongoDbService
{
    private readonly IMongoClient _client;

    public MongoDbService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB") 
            ?? throw new InvalidOperationException("MongoDB connection string not found.");
        
        _client = new MongoClient(connectionString);
    }

    public IMongoDatabase GetDatabase(string databaseName = "MyMvcAppDB")
    {
        return _client.GetDatabase(databaseName);
    }

    public IMongoClient GetClient()
    {
        return _client;
    }
}
