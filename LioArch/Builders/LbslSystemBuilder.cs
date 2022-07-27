//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using LioArch.Entities;
//using LioArch.Modeling;
//using LioArch.Persistence;
//using LioArch.Persistence.Interface;
//using MongoDB.Driver;
//using NLog;
//using Structurizr;
//using Structurizr.Api;
//using Structurizr.Documentation;

//namespace LioArch.Builders
//{
//   public class LbslSystemBuilder
//    {
//        private static ILogger Log = LogManager.GetCurrentClassLogger();
        
//        private static string _templateName = @"LbslSystemBuilder";
//        private static string _templateDescription = @"S2LR000.#??? system";

//        private IMongoClient _mongoClient;
//        private IMongoDatabase _db;

//        private IWorkspaceRepoAccess _workplaceRepo;
//        private IContainersRepository _containers;

//        private SoftwareSystem _builtSoftwareSystem;
//        private SoftwareSystemRepository _softwareRepo;
        
//        public LbslSystemBuilder(IMongoClient mongoClient, IMongoDatabase db, IWorkspaceRepoAccess workplaceRepo, IContainersRepository containers, SoftwareSystem builtSoftwareSystem, SoftwareSystemRepository softwareRepo)
//        {
//            _mongoClient = mongoClient;
//            _db = db;
//            _workplaceRepo = workplaceRepo;
//            _containers = containers;
//            _builtSoftwareSystem = builtSoftwareSystem;
//            _softwareRepo = softwareRepo;
//        }

//        public LbslSystemBuilder()//(IMongoClient mongoClient, IMongoDatabase db)
//        {
//            //_mongoClient = mongoClient;
//            //_db = db;

//            _mongoClient = new MongoClient("mongodb://localhost:27017");
//            _db = _mongoClient.GetDatabase(@"lioarch-lbsl");
//            _containers = new ContainersRepository(_mongoClient);
//        }

//        public Tuple<int, string> StructurizePersistedArchModel(int workspaceId)
//        {
//            Workspace workspace = new WorkspaceRepository().GetWorkspace();
//            workspace.Model.Enterprise = new Enterprise("Lio Product SILIO50");

//            var model = workspace.Model;
//            var views = workspace.Views;
//            var documentation = workspace.Documentation;

//            var systems = _db.GetCollection<LioSoftwareSystem>(nameof(LioSoftwareSystems));
//            var containers = _db.GetCollection<LioContainer>(nameof(LioContainers));
//            var deploymentNodes = _db.GetCollection<LioDeploymentNode>(nameof(LioDeploymentNodes));
//            var personas = _db.GetCollection<LioPersona>(nameof(LioPersonas));

//            var allSystems = systems.FindSync(Builders<LioSoftwareSystem>.Filter.Empty).ToList();
//            var allContainers = containers.FindSync(Builders<LioContainer>.Filter.Empty).ToList();
//            var allDeploymentNodes = deploymentNodes.FindSync(Builders<LioDeploymentNode>.Filter.Empty).ToList();
//            var allPersonas = personas.FindSync(Builders<LioPersona>.Filter.Empty).ToList();

//            foreach (var system in allSystems)
//            {
//                model.AddSoftwareSystem(
//                    FromString(system.Location),
//                    system.Id,
//                    system.Id);
//            }

//            _builtSoftwareSystem = model.GetSoftwareSystemWithName("LioSystem");
//            if( _builtSoftwareSystem == null ) return new Tuple<int, string>(1010, "LioSystem software system not found");

//            foreach (var container in allContainers)
//            {
//                var cont = _builtSoftwareSystem.AddContainer(
//                    container.CanonicalName,
//                    container.Description,
//                    container.Technology);

//                if (cont.Name.Equals("LioDB"))
//                {
//                    cont.AddTags("DB");
//                }
//            }

//            foreach (var system in allSystems)
//            {
//                var modeledSystem = model.GetSoftwareSystemWithName(system.Id);

//                foreach (var lioRelation in system.RelatedWith)
//                {
//                    modeledSystem.Uses(
//                        model.GetSoftwareSystemWithName(lioRelation.Target),
//                        lioRelation.Name);
//                }
//            }

//            foreach (var container in allContainers)
//            {
//                var modeledContainer = _builtSoftwareSystem.GetContainerWithName(container.Id);

//                if (modeledContainer!= null && modeledContainer.CanonicalName.Equals("LioDB"))
//                {
//                    modeledContainer.AddTags("DB");
//                }


//                foreach (var lioRelation in container.Uses)
//                {
//                    var destination = _builtSoftwareSystem.GetContainerWithName(lioRelation.Target);

//                    if (destination != null)
//                    {
//                        modeledContainer.Uses(
//                            _builtSoftwareSystem.GetContainerWithName(lioRelation.Target),
//                            lioRelation.Name);
//                    }
//                    else
//                    {
//                        Log.Error($"Destination element not found - name={lioRelation.Target}");
//                    }

                    
//                }
//            }

//            foreach (var modeledPersona in allPersonas)
//            {
//                var addedPerson = model.AddPerson(Location.Internal, modeledPersona.CanonicalName, modeledPersona.Description);
//                addedPerson.AddProperty("UserName", "Employee");

//                foreach (var uses in modeledPersona.Uses)
//                {
//                    var container =_builtSoftwareSystem.GetContainerWithName(uses.Target);
//                    if (container != null)
//                    {
//                        addedPerson.Uses(container, uses.Action);
//                    }

//                    if (uses.Target.Equals("LioSystem"))
//                    {
//                        addedPerson.Uses(_builtSoftwareSystem, uses.Action);
                        
//                    }
                    
                    
//                }
                

//            }

//            model.AddImplicitRelationships();


//            /*
//             * ................................................................................ Level 1 System Context View
//             */
//            var contextView = views.CreateSystemContextView(_builtSoftwareSystem,
//                "SystemContext",
//                "Overview system context G1 ");


//            contextView.AddAllSoftwareSystems();
//            contextView.AddAllPeople();
//            contextView.EnterpriseBoundaryVisible = true;
//            contextView.EnableAutomaticLayout();


//            /*
//             * ..................................................................................... Level 2 LioContainers View
//             */

//            var containerView = views.CreateContainerView(_builtSoftwareSystem, "LioSystemContainers",
//                "Shows all containers in analytic system");
            
//            containerView.EnableAutomaticLayout();
//            containerView.AddAllPeople();
//            containerView.AddAllContainers();
//            //containerView.AddNearestNeighbours(biSystem);
//            //containerView.AddAnimation(biSystem);


//            try
//            {

//                /*
//                 *  DEPLOYMENT is being build here....
//                 */
//                foreach (var deploymentNode in allDeploymentNodes)
//                {
//                    var createdNode = model.AddDeploymentNode(
//                        deploymentNode.Id, deploymentNode.DisplayName, deploymentNode.Technology);

//                    foreach (var containerRelation in deploymentNode.Containers)
//                    {
//                        var cont = _builtSoftwareSystem.GetContainerWithName(containerRelation.CanonicalName);
//                        if (cont == null)
//                        {
//                            Log.Warn($"Container {containerRelation.CanonicalName} is NOT defined!");
//                        }
//                        else
//                        {
//                            createdNode.Add(_builtSoftwareSystem.GetContainerWithName(containerRelation.CanonicalName));
//                        }
//                    }
//                }

//                /*
//                 * ..................................................................................... Deployment View
//                 */
//                var deploymentView = views.CreateDeploymentView(_builtSoftwareSystem, "LioDeployment",
//                    "Deployment Model in LioSystem");
//                deploymentView.EnableAutomaticLayout();
//                //deploymentView.AddAllDeploymentNodes();
//                //deploymentView.Add(_builtSoftwareSystem.Relationships.First());
//                deploymentView.Environment = "win-x64";
//                deploymentView.SoftwareSystem = _builtSoftwareSystem;

//                foreach (var modelDeploymentNode in model.DeploymentNodes)
//                {
//                    deploymentView.Add(modelDeploymentNode);
//                }

//                deploymentView.EnableAutomaticLayout();
//                deploymentView.AddAllDeploymentNodes();
//                deploymentView.Add(_builtSoftwareSystem.Relationships.First());
//            }
//            catch (Exception any)
//            {
//                Log.Error($"Exceptions");
//                throw;
//            }

//            //BuildViewpointsAndPerspectiveDocumentation(workspace);

//            // add some documentation


//            Documentation docs = new Documentation(model);
//            docs.Hydrate();

//            StructurizrDocumentationTemplate template = new StructurizrDocumentationTemplate(workspace);

//            var lioSystemContext = template.AddContextSection(_builtSoftwareSystem, Format.Markdown,
//                "G1 Backend Services - System Context Level 1\n" +
//                "\n" +
//                "![](embed:SystemContext)");
//            var containersContext = template.AddContainersSection(_builtSoftwareSystem, Format.Markdown,
//                "Backend applications" + "\n" +
//                "![](embed:LioSystemContainers)");

//            var deploymentSection = template.AddDeploymentSection(_builtSoftwareSystem, Format.Markdown,
//                "LioSystem Deployment model" +
//                "![](embed:LioDeployment)");

//            var functionalSection =
//                template.AddFunctionalOverviewSection(_builtSoftwareSystem, Format.Markdown, "#Functional Overview");

//            var supportSection =
//                template.AddOperationAndSupportSection(_builtSoftwareSystem, Format.Markdown, "#Operation and Support");

//            var someSection = template.AddSection(_builtSoftwareSystem, "LioSystem", Format.Markdown, "Lio system section");


            
//            template.AddSoftwareArchitectureSection(_builtSoftwareSystem, Format.Markdown,
//                "Lio Architecture");

//            template.AddDecisionLogSection(_builtSoftwareSystem, Format.Markdown, "ADR Backlog #add MD");


//            List<FileSystemInfo> decisionsMds = new List<FileSystemInfo>();

//            foreach (string entry in Directory.GetFiles(@".\Documentation\adr"))
//            {
//                decisionsMds.Add(new FileInfo(entry));
//            }

            

//            //template.AddDecisionLogSection(_builtSoftwareSystem, Format.Markdown, )
            

//            //template.AddComponentsSection(amwsGateway, Format.Markdown, "Amws Gateway component");

//            // add some styling
//            Styles styles = views.Configuration.Styles;
//            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
//            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
//            styles.Add(new ElementStyle(Tags.Element) { Color = "#ffffff" });
//            styles.Add(new ElementStyle(Tags.Container) { Background = "#08427b", Color = "#ffffff" });
//            styles.Add(new ElementStyle(Tags.DeploymentNode) { Background = "#08425b", Color = "#ffffff" });
//            styles.Add(new ElementStyle("DB") { Shape = Shape.Cylinder });

//            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
//            structurizrClient.PutWorkspace(WorkspaceId, workspace);

//            return new Tuple<int, string>(0, "OK");
//        }

//        private void BuildViewpointsAndPerspectiveDocumentation(Workspace workspace)
//        {
//            ViewpointsAndPerspectivesDocumentation template = new ViewpointsAndPerspectivesDocumentation(workspace);

//            DirectoryInfo documentationRoot = new DirectoryInfo("Documentation" + Path.DirectorySeparatorChar +
//                                                                "viewpointsandperspectives" + Path.DirectorySeparatorChar +
//                                                                "markdown");

//            var fileplace = new FileInfo(Path.Combine(documentationRoot.FullName, "01-introduction.md"));

//            template.AddIntroductionSection(_builtSoftwareSystem,
//                new FileInfo(Path.Combine(documentationRoot.FullName, "01-introduction.md")));
//            template.AddGlossarySection(_builtSoftwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "02-glossary.md")));
//        }


//        private static Location FromString(string locationString)
//        {
//            return (locationString.Equals("Internal")) ? Location.Internal : Location.External;
//        }

//        public static SoftwareSystem CreateLioSystemInModel(ref Model model)
//        {
//            return model.AddSoftwareSystem(Location.Internal, _templateName, _templateDescription);
//        }

//        private void MakeOne(SoftwareSystem lioSystem)
//        {
//            var workspace = new WorkspaceRepository().GetWorkspace();

//            ViewpointsAndPerspectivesDocumentation template = new ViewpointsAndPerspectivesDocumentation(workspace);

//            DirectoryInfo documentationRoot = new DirectoryInfo("Documentation" + Path.DirectorySeparatorChar + "viewpointsandperspectives" + Path.DirectorySeparatorChar + "markdown");

//            var fileplace = new FileInfo(Path.Combine(documentationRoot.FullName, "01-introduction.md"));

//            template.AddIntroductionSection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "01-introduction.md")));
//            template.AddGlossarySection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "02-glossary.md")));
//            //template.AddSystemStakeholdersAndRequirementsSection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "03-system-stakeholders-and-requirements.md")));
//            //template.AddArchitecturalForcesSection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "04-architectural-forces.md")));
//            //template.AddArchitecturalViewsSection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "05-architectural-views")));
//            //template.AddSystemQualitiesSection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "06-system-qualities.md")));
//            //template.AddAppendicesSection(lioSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "07-appendices.md")));
//        }

//    }
//}