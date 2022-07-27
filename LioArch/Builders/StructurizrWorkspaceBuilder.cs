using LioArch.Persistence.Interface;
using MongoDB.Driver;
using NLog;
using Structurizr;
using Structurizr.Api;
using Structurizr.Core.Util;

namespace LioArch.Builders
{
    public class StructurizrWorkspaceBuilder
    {
        private static ILogger Log = LogManager.GetCurrentClassLogger();

        private IWorkspaceRepoAccess _workspaceRepoAccess;
        // convert to ISoft.. interface for IoC 
        private IMongoDatabase _db;

        public StructurizrWorkspaceBuilder(IWorkspaceRepoAccess workspaceRepoAccess, IMongoDatabase db)
        {
            _workspaceRepoAccess = workspaceRepoAccess;
            _db = db;
        }

        public StructurizrWorkspaceBuilder(IWorkspaceRepoAccess workspaceRepoAccess)
        {
            _workspaceRepoAccess = workspaceRepoAccess;
        }
        
        // Use this one for playground 
        public void BuildAndUpload(long workspaceId) // 59811
        {    
            var systemBuilder = new SoftwareSystemBuilder(_workspaceRepoAccess, _db);
            
            var workspace = _workspaceRepoAccess.GetWorkspace(workspaceId);
            var workspaceTargetInfo = _workspaceRepoAccess.GetTarget(workspaceId);

            
            //// Warning!!! playground mode //////

            var model = workspace.Model;

            var dataCenter = model.AddDeploymentNode("SystemTest", "DataCenter", "Hardware back-bone for VMs",
                "VMWare", 1, DictionaryUtils.Create("Tech=VMWare"));

            //var infranode = dataCenter.AddInfrastructureNode("Infranode", "Infranode description", "techstack");

            var centralCluster = dataCenter.AddDeploymentNode( "CentralCluster", "Kubernetes cluster in CC networking - on-premise",
                "kubernetes, docker-engine", 1, DictionaryUtils.Create("Orchestrator=Kubernetes"));
            
            var integrationLayer = model.AddSoftwareSystem(Location.Internal, "IntegrationLayer",
                "Integration layer on CC API domain based access");

            var lioTraffic = integrationLayer.AddContainer("LioTraffic", "Handling uplink messaging from iBus fleet",
                "dotnet-3.1, cloud-based");

            lioTraffic.AddTags("CloudBased");

            centralCluster.Add(lioTraffic);

            var mqPod = centralCluster.AddDeploymentNode("MqPod", "Replicated Pod for MQ nodes", "kubernetes", 1);
            

            var centralMqContainer = integrationLayer.AddContainer("CentralMessageBroker",
                "Message broking system. Externally accessible", "RabbitMQ");

            var voiceQ = centralMqContainer.AddComponent("VoiceQ", "Queues used for Voice operations", "RabbitMq");

            mqPod.Add(centralMqContainer);


            // Relationships:
            lioTraffic.Uses(voiceQ, "Uplink/Downlink voice messaging");


            // Sections and styling

            var views = workspace.Views;

            var deploymentView = views.CreateDeploymentView(integrationLayer, "IntegrationLayerDeployment",
                "Deployment Model for Integration Layer");
            deploymentView.Environment = "SystemTest";
            deploymentView.PaperSize = PaperSize.A0_Landscape;
            deploymentView.EnableAutomaticLayout(RankDirection.TopBottom, 300, 300, 300, false);
            //deploymentView.AddAllDeploymentNodes();
            deploymentView.Add(dataCenter);
            deploymentView.Add(centralCluster);
            deploymentView.Add(mqPod);
            

            //deploymentView.Add(_builtSoftwareSystem.Relationships.First());
            //deploymentView.Environment = "win-x64";
            //deploymentView.SoftwareSystem = _softwareBeingBuilt;

            //foreach (var modelDeploymentNode in model.DeploymentNodes)
            //{
            //    deploymentView.Add(modelDeploymentNode);
            //}

            //deploymentView.EnableAutomaticLayout();
            //deploymentView.AddAllDeploymentNodes();
            //deploymentView.Add(_softwareBeingBuilt.Relationships.First());


            // add some styling
            //Styles styles = views.Configuration.Styles;
            //styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            //styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            //styles.Add(new ElementStyle(Tags.Element) { Color = "#ffffff" });
            //styles.Add(new ElementStyle(Tags.Container) { Background = "#08427b", Color = "#ffffff" });
            //styles.Add(new ElementStyle(Tags.DeploymentNode) { Background = "#08425b", Color = "#ffffff" });
            //styles.Add(new ElementStyle("DB") { Shape = Shape.Cylinder });

            //workspace.Hydrate();

            var structurizrClient = new StructurizrClient(workspaceTargetInfo.ApiKey, workspaceTargetInfo.ApiSecret);
            structurizrClient.PutWorkspace(workspaceId, workspace);
            

        }
    }
}