using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Catalog.Api.Entities
{
    public class TestProduct 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TestCategory { get; set; }
        public string TestName { get; set; }
    }
}
