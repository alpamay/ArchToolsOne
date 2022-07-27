using System;
using System.IO;
using LioArch.Entities;
using LioArch.Modeling;
using MongoDB.Driver;
using NLog;

namespace LioArch.Persistence
{
    public class SoftwareSystemRepository
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IMongoDatabase _db;
        private readonly ModelLoadingSource _modelSource;

        public SoftwareSystemRepository(IMongoClient mongoClient, ModelLoadingSource modelSource)
        {
            _modelSource = modelSource;
            _db = mongoClient.GetDatabase(modelSource.DbName);

            if (_db != null)
            {
                Log.Info($"SoftwareSystem repository for {modelSource.Identification} => MongoDb database {modelSource.DbName} connected!");
            }
        }

        public void EnsureCreated()
        {
            var softwareCollection = _db.GetCollection<LioSoftwareSystem>(nameof(LioSoftwareSystem));
            var lioContainers = _db.GetCollection<LioContainer>(nameof(LioContainers));

            if (softwareCollection == null)
            {
                _db.CreateCollection(nameof(LioSoftwareSystems));
            }

            if (lioContainers == null)
            {
                _db.CreateCollection(nameof(LioContainers));
            }

            if (_db.GetCollection<LioDeploymentNode>(nameof(LioDeploymentNodes)) == null)
            {
                _db.CreateCollection(nameof(LioDeploymentNodes));
            }

            if (_db.GetCollection<LioPersona>(nameof(LioPersonas)) == null)
            {
                _db.CreateCollection(nameof(LioPersonas));
            }


        }

        public void ReloadSoftwareSystems()
        {
            _db.DropCollection(nameof(LioSoftwareSystems));
            _db.CreateCollection(nameof(LioSoftwareSystems));
            var systems = _db.GetCollection<LioSoftwareSystem>(nameof(LioSoftwareSystems));

            try
            {
                using var reader = File.OpenText(Path.Combine(_modelSource.FolderPath, "software-systems.json"));
                var softwareSystems = LioSoftwareSystems.FromJson(reader.ReadToEnd());
                systems.InsertMany(softwareSystems);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void ReloadContainers()
        {
            _db.DropCollection(nameof(LioContainers));
            _db.CreateCollection(nameof(LioContainers));
            var containers = _db.GetCollection<LioContainer>(nameof(LioContainers));

            try
            {
                using var reader = File.OpenText(Path.Combine(_modelSource.FolderPath, "containers.json"));
                var elements = LioContainers.FromJson(reader.ReadToEnd());
                containers.InsertMany(elements);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void ReloadComponents()
        {
            _db.DropCollection(nameof(LioComponents));
            _db.CreateCollection(nameof(LioComponents));
            var components = _db.GetCollection<LioComponent>(nameof(LioComponents));

            try
            {
                using var reader = File.OpenText(Path.Combine(_modelSource.FolderPath, "components.json"));
                components.InsertMany(LioComponents.FromJson(reader.ReadToEnd()));
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        public void ReloadDeploymentNodes()
        {
            _db.DropCollection(nameof(LioDeploymentNodes));
            _db.CreateCollection(nameof(LioDeploymentNodes));
            var containers = _db.GetCollection<LioDeploymentNode>(nameof(LioDeploymentNodes));

            try
            {
                using var reader = File.OpenText(Path.Combine(_modelSource.FolderPath, "deployment-nodes.json"));
                var elements = LioDeploymentNodes.FromJson(reader.ReadToEnd());
                containers.InsertMany(elements);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void ReloadPersonas()
        {
            _db.DropCollection(nameof(LioPersonas));
            _db.CreateCollection(nameof(LioPersonas));
            var containers = _db.GetCollection<LioPersona>(nameof(LioPersonas));

            try
            {
                using var reader = File.OpenText(Path.Combine(_modelSource.FolderPath, "personas.json"));
                var elements = LioPersonas.FromJson(reader.ReadToEnd());
                containers.InsertMany(elements);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

    }
}