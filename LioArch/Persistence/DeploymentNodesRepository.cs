using LioArch.Modeling;
using LioArch.Persistence.Interface;
using MongoDB.Driver;
using NLog;

namespace LioArch.Persistence
{
    public class DeploymentNodesRepository : IDeploymentNodesRepository
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IMongoDatabase _db;

        public DeploymentNodesRepository( IMongoClient mongoDbClient)
        {
            _db = mongoDbClient.GetDatabase(@"lioarch");

            if (_db != null)
            {
                Log.Info("DeploymentNode repository => MongoDb node connected!");
            }
        }

        public LioDeploymentNode GetByIdString(string idString) =>
            _db.GetCollection<LioDeploymentNode>(nameof(LioDeploymentNode))
                .Find(Builders<LioDeploymentNode>.Filter.Eq(deploymentNode => deploymentNode.Id, idString)).Single();
    }
}