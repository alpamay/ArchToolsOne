using LioArch.Container;
using LioArch.Entities;
using LioArch.Persistence.Interface;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NLog;

namespace LioArch.Persistence
{
    public class ContainersRepository : IContainersRepository
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IMongoDatabase _db;

        public ContainersRepository(IMongoClient mongoDbClient)
        {
            _db = mongoDbClient.GetDatabase(@"lioarch");

            if (_db != null)
            {
                Log.Info("Containers repository => MongoDb node connected!");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canonicalName"></param>
        /// <exception cref="EmptyFilterDefinition{LioContainer}"></exception>
        /// <returns></returns>
        public LioContainer GetByName(string canonicalName) =>
            _db.GetCollection<LioContainer>(nameof(LioContainers))
                .Find(Builders<LioContainer>.Filter.Eq(cont => cont.CanonicalName, canonicalName))
                .Single();
       
    }
}