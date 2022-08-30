using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ReminiscenceBot.Models
{
    /// <summary>
    /// MongoDB entity base class, which contains a field to store the MongoDB id.
    /// </summary>
    public abstract record BaseMongoItem
    {
        /// <summary>
        /// This field is used by the MongoDB driver to (de)serialize MongoDB documents.
        /// </summary>
        [BsonId, BsonIgnoreIfDefault]
        private ObjectId _mongoId;
    } 
}
