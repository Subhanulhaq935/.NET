using MongoDB.Driver;

namespace MyMvcApp.Data;

public interface IMongoDbService
{
    IMongoDatabase GetDatabase(string databaseName = "MyMvcAppDB");
    IMongoClient GetClient();
}
