using System;
using System.IO;
using System.Linq;
using LioArch.Entities;
using LioArch.Modeling;
using LioArch.Persistence;
using LioArch.Persistence.Interface;
using MongoDB.Driver;
using NLog;
using Structurizr;
using Structurizr.Api;
using Structurizr.Documentation;

namespace LioArch.Builders
{
    public class LioDocumentationBuilder
    {
        public string BuildAndReturnJsonFormat()
        {

            return "{}";
        }
    }

    public class LioSystemBuilder
    {
        private static ILogger Log = LogManager.GetCurrentClassLogger();
        /*
         * Structurizr API connector config (keep in secret in production)
         */
        private const long WorkspaceId = 59811;
        private const string ApiKey = "8023ed9a-f07a-4ade-9bb8-35a957d8af17";
        private const string ApiSecret = "c7e4ea64-a449-49e8-b8fb-6b5bab556ca9";
        // ------------------------------------------------------------------
        
        private static string _templateName = @"LioSystemBuilder";
        private static string _templateDescription = @"SILIO50 system";

        private IMongoClient _mongoClient;
        private IMongoDatabase _db;

        private IWorkspaceRepoAccess _workplaceRepo;
        private IContainersRepository _containers;

        private SoftwareSystem _builtSoftwareSystem;
        private SoftwareSystemRepository _softwareRepo;
        
        public LioSystemBuilder(IMongoClient mongoClient, IMongoDatabase db, IWorkspaceRepoAccess workplaceRepo, IContainersRepository containers, SoftwareSystem builtSoftwareSystem, SoftwareSystemRepository softwareRepo)
        {
            _mongoClient = mongoClient;
            _db = db;
            _workplaceRepo = workplaceRepo;
            _containers = containers;
            _builtSoftwareSystem = builtSoftwareSystem;
            _softwareRepo = softwareRepo;
        }

        public LioSystemBuilder()//(IMongoClient mongoClient, IMongoDatabase db)
        {
            //_mongoClient = mongoClient;
            //_db = db;

            _mongoClient = new MongoClient("mongodb://localhost:27017");
            _db = _mongoClient.GetDatabase(@"lioarch");
            _containers = new ContainersRepository(_mongoClient);
        }

        public Tuple<int, string> StructurizePersistedArchModel(int workspaceId)
        {
            Workspace workspace = new WorkspaceRepository().GetWorkspace(workspaceId);
            //workspace.Model.Enterprise = new Enterprise("Enterprise name for LIO model");

            var model = workspace.Model;
            var views = workspace.Views;
            var documentation = workspace.Documentation;
            
            var systems = _db.GetCollection<LioSoftwareSystem>(nameof(LioSoftwareSystems));
            var containers = _db.GetCollection<LioContainer>(nameof(LioContainers));
            var components = _db.GetCollection<LioComponent>(nameof(LioComponents));
            var deploymentNodes = _db.GetCollection<LioDeploymentNode>(nameof(LioDeploymentNodes));
            var personas = _db.GetCollection<LioPersona>(nameof(LioPersonas));
            
            var allSystems = systems.FindSync(Builders<LioSoftwareSystem>.Filter.Empty).ToList();
            var allContainers = containers.FindSync(Builders<LioContainer>.Filter.Empty).ToList();
            var allComponents = components.FindSync(Builders<LioComponent>.Filter.Empty).ToList();
            var allDeploymentNodes = deploymentNodes.FindSync(Builders<LioDeploymentNode>.Filter.Empty).ToList();
            var allPersonas = personas.FindSync(Builders<LioPersona>.Filter.Empty).ToList();
            
            foreach (var system in allSystems)
            {
                model.AddSoftwareSystem(
                    FromString(system.Location),
                    system.Id,
                    system.Id);
            }

            
            _builtSoftwareSystem = model.GetSoftwareSystemWithName("LioSystem");

            if( _builtSoftwareSystem == null ) return new Tuple<int, string>(1010, "LioSystem software system not found");
            
            foreach (var system in allSystems)
            {
                var modeledSystem = model.GetSoftwareSystemWithName(system.Id);

                foreach (var lioRelation in system.RelatedWith)
                {
                    modeledSystem.Uses(
                        model.GetSoftwareSystemWithName(lioRelation.Target),
                        lioRelation.Name);
                }
            }

            foreach (var container in allContainers)
            {
                var cont = _builtSoftwareSystem.AddContainer(
                    container.CanonicalName,
                    container.Description,
                    container.Technology);

                cont.AddTags(container.Tag.Split(","));

                if (container.Components != null)
                {
                    foreach (var componentName in container.Components)
                    {
                        var component = allComponents.First(comp => comp.CanonicalName == componentName);

                        if (component == null)
                        {
                            Log.Error($"Cannot find component: {componentName}");
                            continue;
                        }
                        cont.AddComponent(component.CanonicalName, component.Description, component.Technology);
                    }
                }
            }

            foreach (var container in allContainers)
            {
                var modeledContainer = _builtSoftwareSystem.GetContainerWithName(container.Id);

                foreach (var lioRelation in container.Uses)
                {
                    var destination = _builtSoftwareSystem.GetContainerWithName(lioRelation.Target);

                    if (destination != null)
                    {
                        modeledContainer.Uses(
                            _builtSoftwareSystem.GetContainerWithName(lioRelation.Target),
                            lioRelation.Name);
                    }
                    else
                    {
                        Log.Error($"Destination element not found - name={lioRelation.Target}");
                    }
                    
                }
            }

            foreach (var modeledPersona in allPersonas)
            {
                var addedPerson = model.AddPerson(Location.Internal, modeledPersona.CanonicalName, modeledPersona.Description);
                
                foreach (var uses in modeledPersona.Uses)
                {
                    var container =_builtSoftwareSystem.GetContainerWithName(uses.Target);
                    if (container != null)
                    {
                        addedPerson.Uses(container, uses.Action);
                    }

                    var usedSystem = model.GetSoftwareSystemWithName(uses.Target);
                    if (usedSystem != null)
                    {
                        addedPerson.Uses(usedSystem, uses.Action);
                    }
                }
            }

            /*
             * ................................................................................ Level 1 System Context View
             */
            var contextView = views.CreateSystemContextView(_builtSoftwareSystem,
                "SystemContext",
                "Overview system context G1 ");

            contextView.PaperSize = PaperSize.A0_Landscape;
            contextView.AddAllPeople();
            contextView.AddAllSoftwareSystems();
            contextView.EnterpriseBoundaryVisible = true;
            contextView.EnableAutomaticLayout();
            //contextView.EnableAutomaticLayout(RankDirection.TopBottom, 300, 300, 300, true);


            SystemLandscapeView systemLandscapeView = views.CreateSystemLandscapeView("LioSystemContext", "Enterprise context for LIO");
            systemLandscapeView.PaperSize = PaperSize.A0_Landscape;
            systemLandscapeView.AddAllElements();
            systemLandscapeView.EnableAutomaticLayout();
            systemLandscapeView.EnterpriseBoundaryVisible = true;
            

            /*
             * ..................................................................................... Level 2 LioContainers View
             */

            var containerView = views.CreateContainerView(_builtSoftwareSystem, "LioSystemContainers",
                "Shows all containers in analytic system");

            containerView.PaperSize = PaperSize.A0_Landscape;
            //containerView.ExternalSoftwareSystemBoundariesVisible = true;
            containerView.EnableAutomaticLayout(RankDirection.TopBottom, 300, 300, 300, true);
            containerView.AddAllPeople();
            containerView.AddAllContainers();
            
            //containerView.AddNearestNeighbours(biSystem);
            //containerView.AddAnimation(biSystem);


            /*
             * ....................................................................................... Level 3 Component view
             */

            foreach (var container in _builtSoftwareSystem.Containers)
            {
                if (container.Components.Any())
                {
                    var componentView = views.CreateComponentView(container,
                        $"{container.CanonicalName}ComponentView",
                        $"Component view of {container.CanonicalName}");
                    componentView.PaperSize = PaperSize.A4_Landscape;
                    componentView.EnableAutomaticLayout();
                    componentView.AddAllComponents();
                }
            }
            
            try
            {

                /*
                 *  DEPLOYMENT is being build here....
                 */
                foreach (var deploymentNode in allDeploymentNodes)
                {
                    var createdNode = model.AddDeploymentNode(
                        deploymentNode.Id, deploymentNode.DisplayName, deploymentNode.Technology);

                    foreach (var containerRelation in deploymentNode.Containers)
                    {
                        var cont = _builtSoftwareSystem.GetContainerWithName(containerRelation.CanonicalName);
                        if (cont == null)
                        {
                            Log.Warn($"Container {containerRelation.CanonicalName} is NOT defined!");
                        }
                        else
                        {
                            createdNode.Add(_builtSoftwareSystem.GetContainerWithName(containerRelation.CanonicalName));
                        }
                    }
                }

                /*
                 * ..................................................................................... Deployment View
                 */
                var deploymentView = views.CreateDeploymentView(_builtSoftwareSystem, "LioDeployment",
                    "Deployment Model in LioSystem");

                deploymentView.PaperSize = PaperSize.A0_Landscape;
                deploymentView.EnableAutomaticLayout(RankDirection.TopBottom, 300, 300, 300, true);
                //deploymentView.AddAllDeploymentNodes();
                deploymentView.Environment = "win-x64";
                deploymentView.SoftwareSystem = _builtSoftwareSystem;

                foreach (var modelDeploymentNode in model.DeploymentNodes)
                {
                    deploymentView.Add(modelDeploymentNode);
                }

            }
            catch (Exception any)
            {
                Log.Error($"Exceptions");
                throw;
            }



            BuildViewpointsAndPerspectiveDocumentation(workspace);

            // add some documentation
            StructurizrDocumentationTemplate template = new StructurizrDocumentationTemplate(workspace);
            
            var lioSystemContext = template.AddContextSection(_builtSoftwareSystem, Format.Markdown,
                "G1 Backend Services - System Context Level 1\n" +
                "\n" +
                "![](embed:SystemContext)" +
                "![](embed:LioSystemContext)");
            
            var containersContext = template.AddContainersSection(_builtSoftwareSystem, Format.Markdown,
                "Backend applications" + "\n" +
                "![](embed:LioSystemContainers)");

            var deploymentSection = template.AddDeploymentSection(_builtSoftwareSystem, Format.Markdown,
                "LioSystem Deployment model" +
                "![](embed:LioDeployment)");

            var someSection = template.AddSection(_builtSoftwareSystem, "LioSystem", Format.Markdown, "Lio system section");
            
            
            template.AddSoftwareArchitectureSection(_builtSoftwareSystem, Format.Markdown,
                "Lio Architecture");

            template.AddDecisionLogSection(_builtSoftwareSystem, Format.Markdown, "ADR Backlog #add MD");
            template.AddFunctionalOverviewSection(_builtSoftwareSystem, Format.Markdown, "Runtime functional overview");
            
            // add some styling
            Styles styles = views.Configuration.Styles;
            //styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            //styles.Add(new ElementStyle(Tags.Element) { Color = "#ffffff" });
            //styles.Add(new ElementStyle(Tags.Container) { Background = "#08427b", Color = "#ffffff" });
            //styles.Add(new ElementStyle(Tags.DeploymentNode) { Background = "#08425b", Color = "#ffffff" });
            styles.Add(new ElementStyle("DB") { Shape = Shape.Cylinder });
            styles.Add(new ElementStyle("MQ") {Shape = Shape.Hexagon});

            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);

            return new Tuple<int, string>(0, "OK");
        }

        private void BuildArc42Documentation(Workspace workspace)
        {
            Arc42DocumentationTemplate template = new Arc42DocumentationTemplate(workspace);

            var buildingBlockSection = template.AddBuildingBlockViewSection(_builtSoftwareSystem, Format.Markdown,
                @"BuildingBlock section goes here");
        }

        private void BuildViewpointsAndPerspectiveDocumentation(Workspace workspace)
        {
            ViewpointsAndPerspectivesDocumentation template = new ViewpointsAndPerspectivesDocumentation(workspace);

            DirectoryInfo documentationRoot = new DirectoryInfo("Documentation" + Path.DirectorySeparatorChar +
                                                                "viewpointsandperspectives" + Path.DirectorySeparatorChar +
                                                                "markdown");

            var fileplace = new FileInfo(Path.Combine(documentationRoot.FullName, "01-introduction.md"));

            template.AddIntroductionSection(_builtSoftwareSystem,
                new FileInfo(Path.Combine(documentationRoot.FullName, "01-introduction.md")));
            template.AddGlossarySection(_builtSoftwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "02-glossary.md")));
        }


        private static Location FromString(string locationString)
        {
            return (locationString.Equals("Internal")) ? Location.Internal : Location.External;
        }

        public static SoftwareSystem CreateLioSystemInModel(ref Model model)
        {
            return model.AddSoftwareSystem(Location.Internal, _templateName, _templateDescription);
        }

        private void MakeOne(SoftwareSystem lioSystem, long workstationId)
        {
            var workspace = new WorkspaceRepository().GetWorkspace(workstationId);

            ViewpointsAndPerspectivesDocumentation template = new ViewpointsAndPerspectivesDocumentation(workspace);

            DirectoryInfo documentationRoot = new DirectoryInfo("Documentation" + Path.DirectorySeparatorChar + "viewpointsandperspectives" + Path.DirectorySeparatorChar + "markdown");

            var fileplace = new FileInfo(Path.Combine(documentationRoot.FullName, "01-introduction.md"));

            template.AddIntroductionSection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "01-introduction.md")));
            template.AddGlossarySection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "02-glossary.md")));
            //template.AddSystemStakeholdersAndRequirementsSection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "03-system-stakeholders-and-requirements.md")));
            //template.AddArchitecturalForcesSection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "04-architectural-forces.md")));
            //template.AddArchitecturalViewsSection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "05-architectural-views")));
            //template.AddSystemQualitiesSection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "06-system-qualities.md")));
            //template.AddAppendicesSection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "07-appendices.md")));
        }

    }
}