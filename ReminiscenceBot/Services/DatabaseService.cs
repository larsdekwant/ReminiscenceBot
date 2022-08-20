using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Bson;

namespace ReminiscenceBot.Services
{    
    public class DatabaseService
    {
        private IMongoDatabase _db;

        public DatabaseService(string name)
        {
            var client = new MongoClient("mongodb://admin:password@mongodb");
            _db = client.GetDatabase(name);
        }

        public void InsertDocument<T>(string table, T document)
        {
            var collection = _db.GetCollection<T>(table);
            collection.InsertOne(document);
        }

        public List<T> LoadAllDocuments<T>(string table)
        {
            var collection = _db.GetCollection<T>(table);

            return collection.Find(new BsonDocument()).ToList();
        }

        public T LoadDocumentById<T>(string table, int id)
        {
            var collection = _db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);

            return collection.Find(filter).First();
        }

        public void UpsertDocument<T>(string table, ulong id, T document)
        {
            var collection = _db.GetCollection<T>(table);
            collection.ReplaceOne(
                Builders<T>.Filter.Eq("Id", id),
                document,
                new ReplaceOptions { IsUpsert = true });
        }

        public void DeleteDocument<T>(string table, ulong id)
        {
            var collection = _db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }
    }
}
