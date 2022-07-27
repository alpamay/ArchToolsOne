using LioArch.Entities;
using LioArch.Persistence.Interface;
using MongoDB.Driver;
using NLog;

namespace LioArch.Persistence
{
    public class LioComponentsRepository : ILioComponentsRepository
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IMongoDatabase _db;

        public LioComponentsRepository(IMongoClient mongoDbClient)
        {
            _db = mongoDbClient.GetDatabase(@"lioarch");

            if (_db != null)
            {
                Log.Info("LioComponent repository => MongoDb node connected!");
            }
        }
        public LioComponent GetByName(string canonicalName) =>
            _db.GetCollection<LioComponent>(nameof(LioComponents))
                .Find(Builders<LioComponent>.Filter.Eq(component => component.CanonicalName, canonicalName)).Single();

    }
}