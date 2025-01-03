using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IoTHub.Models
{
    public class Device
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }
        public required string DeviceName { get; set; }
        public required string DeviceType { get; set; }
        public bool Status { get; set; }
        public required string UserId { get; set; }
    }
}