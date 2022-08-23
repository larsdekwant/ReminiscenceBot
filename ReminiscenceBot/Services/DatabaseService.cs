using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Bson;

using Discord;

namespace ReminiscenceBot.Services
{    
    public class DatabaseService
    {
        private IMongoDatabase _db;

        public DatabaseService(string name)
        {
            var client = new MongoClient("mongodb://admin:password@localhost:27017");
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

        public T? LoadDocument<T>(string table, FilterDefinition<T> filter)
        {
            var collection = _db.GetCollection<T>(table);
            var matches = collection.Find(filter);

            if (matches.CountDocuments() == 0) return default;

            return matches.First();
        }

        public void UpsertDocument<T>(string table, FilterDefinition<T> filter, T document)
        {
            var collection = _db.GetCollection<T>(table);
            collection.ReplaceOne(filter, document, new ReplaceOptions { IsUpsert = true });
        }

        public void DeleteDocument<T>(string table, FilterDefinition<T> filter)
        {
            var collection = _db.GetCollection<T>(table);
            collection.DeleteOne(filter);
        }
    }
}
