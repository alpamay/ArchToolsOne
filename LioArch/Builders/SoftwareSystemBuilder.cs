using System;
using System.Linq;
using LioArch.Entities;
using LioArch.Modeling;
using LioArch.Persistence;
using LioArch.Persistence.Interface;
using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Driver;
using NLog;
using Structurizr;
using Structurizr.Api;

namespace LioArch.Builders
{
    public class SoftwareSystemBuilder
    {
        private static ILogger Log = LogManager.GetCurrentClassLogger();

        private IWorkspaceRepoAccess _workspaceRepoAccess;
        // convert to ISoft.. interface for IoC 
        private readonly IMongoDatabase _db;

        private SoftwareSystem _softwareBeingBuilt;

        public SoftwareSystemBuilder(IWorkspaceRepoAccess workspaceRepoAccess, IMongoDatabase database)
        {
            _workspaceRepoAccess = workspaceRepoAccess;
            _db = database;
        }

        //public SoftwareSystem BuildSoftwareSystemInWorkspace(Workspace workspace)
        //{

        //}

        public Tuple<int, string> StructurizePersistedArchModel(int workspaceId, string focusedSystemName)
        {
            var workspace = _workspaceRepoAccess.GetWorkspace(workspaceId);
            var workspaceTarget = _workspaceRepoAccess.GetTarget(workspaceId);
            // TODO #todo enterprise name from modeling source or jsons
            workspace.Model.Enterprise = new Enterprise("Lio Ecosystem");

            var model = workspace.Model;
            var views = workspace.Views;

            var softwareSystem = BuildSystemInModel(focusedSystemName, model);

            var systemContextView = views.CreateSystemContextView(softwareSystem, workspaceTarget.Name,
                workspaceTarget.Description);
            systemContextView.AddAllSoftwareSystems();
            systemContextView.AddAllPeople();
            systemContextView.EnterpriseBoundaryVisible = true;
            systemContextView.EnableAutomaticLayout();

            // TODO #todo Figure out how to steer detail level of satellite systems. What to include in views.
            // just try out - LbslSystem treat 

            //var satelliteSystemView = views.CreateSystemContextView(model.GetSoftwareSystemWithName("LbslSystem"),
            //    "LbslSystem", "Satellite system");
            //satelliteSystemView.AddAllSoftwareSystems();
            //satelliteSystemView.AddAllPeople();
            ////satelliteSystemView.EnterpriseBoundaryVisible = true;
            //satelliteSystemView.EnableAutomaticLayout();


            var containerView = views.CreateContainerView(softwareSystem, "",
                "Shows all containers in analytic system");

            containerView.AddAllPeople();
            containerView.AddAllContainers();
            //containerView.AddNearestNeighbours(_softwareBeingBuilt);
            //containerView.AddAnimation(_softwareBeingBuilt);
            containerView.EnableAutomaticLayout();


            //var deploymentView = views.CreateDeploymentView(_softwareBeingBuilt, "LioDeployment",
            //    "Deployment Model in LioSystem");
            //deploymentView.SoftwareSystem = _softwareBeingBuilt;
            //deploymentView.Environment = "DemoLab";

            //foreach (var rootNode in model.DeploymentNodes)//.Where(node => !node.Children.Any()))
            //{
            //    deploymentView.Add(rootNode);
            //}

            //deploymentView.EnableAutomaticLayout();

            /*
                 * ..................................................................................... Deployment View
                 */
            var deploymentView = views.CreateDeploymentView(_softwareBeingBuilt, "LioDeployment",
                "Deployment Model in LioSystem");
            deploymentView.EnableAutomaticLayout();
            //deploymentView.AddAllDeploymentNodes();
            //deploymentView.Add(_builtSoftwareSystem.Relationships.First());
            deploymentView.Environment = "win-x64";
            deploymentView.SoftwareSystem = _softwareBeingBuilt;

            foreach (var modelDeploymentNode in model.DeploymentNodes)
            {
                deploymentView.Add(modelDeploymentNode);
            }

            deploymentView.EnableAutomaticLayout();
            deploymentView.AddAllDeploymentNodes();
            deploymentView.Add(_softwareBeingBuilt.Relationships.First());


            // add some styling
            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle(Tags.Element) { Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Container) { Background = "#08427b", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.DeploymentNode) { Background = "#08425b", Color = "#ffffff" });
            styles.Add(new ElementStyle("DB") { Shape = Shape.Cylinder });

            //workspace.Hydrate();

            var structurizrClient = new StructurizrClient(workspaceTarget.ApiKey, workspaceTarget.ApiSecret);
            structurizrClient.PutWorkspace(workspaceId, workspace);

            return new Tuple<int, string>(1000,"Software system built successfully");
        }


        public SoftwareSystem BuildSystemInModel(string focusedSystemName, Model model)
        {
            var systems = _db.GetCollection<LioSoftwareSystem>(nameof(LioSoftwareSystems));
            var containers = _db.GetCollection<LioContainer>(nameof(LioContainers));
            var deploymentNodes = _db.GetCollection<LioDeploymentNode>(nameof(LioDeploymentNodes));
            var personas = _db.GetCollection<LioPersona>(nameof(LioPersonas));

            var allSystems = systems.FindSync(Builders<LioSoftwareSystem>.Filter.Empty).ToList();
            var allContainers = containers.FindSync(Builders<LioContainer>.Filter.Empty).ToList();
            var allDeploymentNodes = deploymentNodes.FindSync(Builders<LioDeploymentNode>.Filter.Empty).ToList();
            var allPersonas = personas.FindSync(Builders<LioPersona>.Filter.Empty).ToList();

            var persistedSoftwareSystem = allSystems.Find(soft => soft.Id.Equals(focusedSystemName));

            // TODO Change DisplayName to real description
            _softwareBeingBuilt = model.AddSoftwareSystem(Location.Internal, persistedSoftwareSystem.Id, persistedSoftwareSystem.DisplayName);

            foreach (var container in allContainers)
            {
                var cont = _softwareBeingBuilt.AddContainer(
                    container.CanonicalName,
                    container.Description,
                    container.Technology);

                if (!string.IsNullOrWhiteSpace(container.Tag))
                {
                    cont.AddTags(container.Tag);
                }
            }

            foreach (var modeledPersona in allPersonas)
            {
                var addedPerson = model.AddPerson(Location.Internal, modeledPersona.CanonicalName,
                    modeledPersona.Description);
                addedPerson.AddProperty("UserName", "Employee");
            }

            foreach (var deploymentNode in allDeploymentNodes)
            {
                var createdNode = model.AddDeploymentNode("DemoLab",
                    deploymentNode.Id, deploymentNode.DisplayName, deploymentNode.Technology);
            }


            ////////////////// relations
            /// ///
            ///
            ///

            //foreach (var lioSoftwareSystem in allSystems)
            //{
            //    var modelSystem = model.GetSoftwareSystemWithName(lioSoftwareSystem.Id);

            //    if (modelSystem == null)
            //    {
            //        Log.Warn($"Software system {lioSoftwareSystem.Id} cannot be found");
            //        continue;
            //    }

            //    foreach (var lioRelation in lioSoftwareSystem.RelatedWith)
            //    {
            //        var childModelSystem = model.GetSoftwareSystemWithName(lioRelation.Target);

            //        if (childModelSystem != null)
            //        {
            //            modelSystem.Uses(childModelSystem, lioRelation.Name);
            //        }
            //        else
            //        {
            //            Log.Warn($"Chils system {lioRelation.Target} cannot be found");
            //        }
            //    }
            //}


            foreach (var deploymentNode in allDeploymentNodes)
            {
                var modelDeploymentNode = model.GetDeploymentNodeWithName(deploymentNode.Id, "DemoLab");

                foreach (var childRelation in deploymentNode.Nodes)
                {
                    var modelChildNode = model.GetDeploymentNodeWithName(childRelation.Id, "DemoLab");

                    if (modelChildNode != null)
                    {
                        modelDeploymentNode.Children.Add(modelChildNode);
                    }
                    else
                    {
                        Log.Warn($"System builder Cannot find deployment node name={childRelation.Id}");
                    }
                }

                foreach (var lioContainerChildRelation in deploymentNode.Containers)
                {
                    var modelContainer = _softwareBeingBuilt.GetContainerWithName(lioContainerChildRelation.CanonicalName);

                    if (modelContainer != null)
                    {
                        modelDeploymentNode.Add(modelContainer);
                    }
                    else
                    {
                        Log.Warn(
                            $"Container {lioContainerChildRelation.CanonicalName} cannot be found in {_softwareBeingBuilt.CanonicalName}");
                    }
                }
            }


            foreach (var persona in allPersonas)
            {
                var modelPersona = model.GetPersonWithName(persona.Id);

                foreach (var lioPersonaUsesRelation in persona.Uses)
                {
                    var usedSoftware = model.GetSoftwareSystemWithName(lioPersonaUsesRelation.Target);
                    if (usedSoftware != null)
                    {
                        modelPersona.Uses(usedSoftware, lioPersonaUsesRelation.Action);

                    }

                    var usedContainer = _softwareBeingBuilt.GetContainerWithName(lioPersonaUsesRelation.Target);
                    if (usedContainer != null)
                    {
                        modelPersona.Uses(usedContainer, lioPersonaUsesRelation.Action);
                    }
                }
            }

            foreach (var lioContainer in allContainers)
            {
                var modelContainer = _softwareBeingBuilt.GetContainerWithName(lioContainer.CanonicalName);

                if (modelContainer != null)
                {
                    foreach (var lioContainerUse in lioContainer.Uses)
                    {
                        var usedContainer = _softwareBeingBuilt.GetContainerWithName(lioContainerUse.Target);

                        if (usedContainer != null)
                        {
                            modelContainer.Uses(usedContainer, lioContainerUse.Name);
                        }
                    }
                }
            }

            return _softwareBeingBuilt;
        }

        private static Location FromString(string locationString)
        {
            return (locationString.Equals("Internal")) ? Location.Internal : Location.External;
        }
    }
}