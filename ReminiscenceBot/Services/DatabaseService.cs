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
        private readonly IMongoDatabase _db;

        public DatabaseService(string name)
        {
            var client = new MongoClient("mongodb://admin:password@localhost:27017");
            _db = client.GetDatabase(name);            
        }

        /// <summary>
        /// Insert a document into the given table
        /// </summary>
        /// <typeparam name="T">The type of the documents that will be loaded</typeparam>
        /// <param name="table">The table to insert the document into</param>
        /// <param name="document">The document to insert into the table</param>
        public void InsertDocument<T>(string table, T document)
        {
            var collection = _db.GetCollection<T>(table);
            collection.InsertOne(document);
        }

        /// <summary>
        /// Loads all documents from the given table that match the filter
        /// </summary>
        /// <typeparam name="T">The type of the documents that will be loaded</typeparam>
        /// <param name="table">The table to load data from</param>
        /// <param name="filter">The filter to select which documents to load</param>
        /// <returns>List of documents, matching the filter, deserialized into the object type</returns>
        public List<T> LoadDocuments<T>(string table, FilterDefinition<T> filter)
        {
            var collection = _db.GetCollection<T>(table);

            return collection.Find(filter).ToList();
        }

        /// <summary>
        /// Loads all documents from the given table
        /// </summary>
        /// <typeparam name="T">The type of the documents that will be loaded</typeparam>
        /// <param name="table">The table to load data from</param>
        /// <returns>List of all documents, deserialized into the object type</returns>
        public List<T> LoadAllDocuments<T>(string table)
        {
            var collection = _db.GetCollection<T>(table);

            return collection.Find(new BsonDocument()).ToList();
        }

        /// <summary>
        /// Inserts the given documents or updates an existing document that matches the filter
        /// </summary>
        /// <typeparam name="T">The type of the documents that will be loaded</typeparam>
        /// <param name="table">The target table to upsert in</param>
        /// <param name="filter">The filter to use when checking for updatable documents</param>
        /// <param name="document">The document to insert or update with</param>
        public void UpsertDocument<T>(string table, FilterDefinition<T> filter, T document)
        {
            var collection = _db.GetCollection<T>(table);
            collection.ReplaceOne(filter, document, new ReplaceOptions { IsUpsert = true });
        }

        /// <summary>
        /// Deletes a single document matching the filter. 
        /// If multiple documents match, only the first match is deleted.
        /// </summary>
        /// <typeparam name="T">The type of the documents that will be loaded</typeparam>
        /// <param name="table">The target table to delete from</param>
        /// <param name="filter">The filter to find the document that should be deleted</param>
        public void DeleteDocument<T>(string table, FilterDefinition<T> filter)
        {
            var collection = _db.GetCollection<T>(table);
            collection.DeleteOne(filter);
        }
    }
}
