using IoTHub.Models;
using MongoDB.Driver;

namespace IoTHub.Data
{
    public interface IMongoDBContext
    {
        IMongoCollection<User> Users { get; }
        IMongoCollection<T> GetCollection<T>(string name);
    }
}