using System.Collections.Generic;
using Core.Data;
using Core.Entity;
using Entity;
using Nest;
using System;
using System.Threading.Tasks;
using System.Linq;
using Core.Config;
using MongoDB.Driver;

namespace Data.Mongo
{
    public class ContentRepository : IRepository<Content, int>
    {
        private const string COLLECTION_NAME = "Content";
        private readonly IMongoCollection<Content> _mongoCollection;

        private readonly IApplicationConfig _applicationConfig;

        public ContentRepository(IApplicationConfig applicationConfig)
        {
            _applicationConfig = applicationConfig;
            _mongoCollection = InitializeMongoCollection(_applicationConfig.ConnectionStrings.Mongo);
        }

        private IMongoCollection<Content> InitializeMongoCollection(string connectionString)
        {
            var mongoClient = new MongoClient(connectionString);
            var mongoDatabase = mongoClient.GetDatabase("ContentDB");
            mongoDatabase.DropCollection(COLLECTION_NAME);
            mongoDatabase.CreateCollection(COLLECTION_NAME);
            var mongoCollection = mongoDatabase.GetCollection<Content>(COLLECTION_NAME);
            return mongoCollection;
        }

        public async Task CreateAsync(IEntity<int> item)
        {
            await _mongoCollection.InsertOneAsync((Content)item);
        }

        public void Delete(int id)
        {
            _mongoCollection.DeleteOne(q => q.Id == id);
        }

        public Content Get(int id)
        {
            var content = _mongoCollection.Find(q => q.Id == id).First();
            return content;
        }

        public IList<Content> GetAll()
        {
            var contents = _mongoCollection.AsQueryable().ToList();
            return contents;
        }

        public void Update(IEntity<int> item)
        {
            _mongoCollection.ReplaceOne(q => q.Id == item.Id, (Content)item);
        }
    }
}
