using System;
using LioArch.Persistence;
using Structurizr;
using Structurizr.Api;
using Structurizr.Documentation;

namespace LioArch.Builders
{
    public static class PlaygroundBuilder
    {
        /*
         * Structurizr API connector config (keep in secret in production)
         */
        private const long WorkspaceId = 59767;
        private const string ApiKey = "869c1c60-b9f0-48a4-89de-336384f35e79";
        private const string ApiSecret = "4beafcaa-459e-4c96-bb0e-326d413159f2";
        // ------------------------------------------------------------------------- disappear that!

        private const string DatabaseTag = "LioDb";

        static void Build(string[] args)
        {
            

            Console.ReadKey();

            var workspace = new WorkspaceRepository().GetWorkspace(44);
            workspace.Model.Enterprise = new Enterprise("Enterprise name for LIO model");

            var model = workspace.Model;
            var views = workspace.Views;
            var documentation = workspace.Documentation;

            var decisionLog = documentation.Decisions;
            var images = documentation.Images;
            var sections = documentation.Sections;

            var deploymentNodes = workspace.Model.DeploymentNodes;

            var people = workspace.Model.People;
            var relationships = workspace.Model.Relationships;
            var softwareSystems = workspace.Model.SoftwareSystems;


            var lioDispatcherUser = model.AddPerson("Dispatcher", "A User of G2 UI Client (3rd level permission)");
            var passenger = model.AddPerson("Passenger", "A common passenger");
            var lioAdminUser = model.AddPerson("LioAdmin", "2nd level permission user");
            var lioSuperAdmin = model.AddPerson("SuperUser", "1st level access - mayor upgrades");




            /*
             * ................................................................. Level 1 SOFTWARE SYSTEM  Definitions
             */

            SoftwareSystem lioSystem = LioSystemBuilder.CreateLioSystemInModel(ref model);
            var biSystem = LioSystemDependencies.CreateBiSystemInModel(ref model);
            var paritySystem = LioSystemDependencies.CreateParitySystemInModel(ref model);
            var lioData = LioSystemDependencies.CreateLioDataSystemInModel(ref model);
            var idrFleet = LioSystemDependencies.CreateIdrFleetSystemInModel(ref model);

            Element elem = lioSystem.Parent;
            


            /*
             * ................................................................................ Level 1 Relationships
             */

            lioData.Delivers(lioAdminUser, "DataSupply packages", "txt\\pl5", InteractionStyle.Asynchronous);

            biSystem.Uses(lioSystem, "Gets data", "txt|csv, logs [offline]");
            biSystem.Uses(idrFleet, "Gets data", "txt/saf [offline]");

            lioSystem.Uses(paritySystem, "Audio functions", "tcp/ip");
            lioSystem.Uses(idrFleet, "Uplink/Downlink traffic", "NLSm protocol");
            lioSystem.Uses(lioData, "Received DS packages", "txt/csv [offline]");

            idrFleet.Uses(lioData, "Data loading", "ddm (???) [offline]");
            idrFleet.Uses(paritySystem, "Register and participate", "sip/poc [parity flavor]");

            passenger.Uses(lioSystem, "Receives Passenger Info");
            lioDispatcherUser.Uses(lioSystem, "Interacts with ClientG2");

            /*
             * ................................................................................ Level 1 System Context View
             */
            var contextView = views.CreateSystemContextView(lioSystem,
                "SystemContext G1",
                "Overview system context G1 ");

            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();
            contextView.AddNearestNeighbours(biSystem);
            contextView.AddAnimation(biSystem);
            contextView.EnterpriseBoundaryVisible = true;
            contextView.EnableAutomaticLayout();


            /*
             * .................................................................................. Level 2 LioContainers Definitions
             */
            var lioDbEnvContainer = lioSystem.AddContainer(
                "LioDb Environment",
                "Core database [state: widely-used, evolution: read-only => migration => deactivation]",
                "c++, c++/cli, .NET 4.8, dotnet-3.1 (partially)");

            lioDbEnvContainer.AddTags(DatabaseTag);


            var g1AdapterService = lioSystem.AddContainer(
                "G1AdapterService",
                "Aggregation host for sophisticated components",
                technology: "dotnet-3.1, .NET 4.8, c++. .NET remoting");

            

            var dialogApiEndpoint = lioSystem.AddContainer(
                "DialogAPI",
                "IIS Application Pool (s)",
                "aspnet mvc, webapi, .NET remoting");

            var lioDcg2Container = lioSystem.AddContainer(
                "LioDCG2Container",
                "Container for LioAdapter",
                "jdk-1.8, .NET 4.8, cpp/clr");

            var actionManager = lioSystem.AddContainer(
                "ActionManager",
                "LioMama action manager",
                "cpp/clr (??), liodb-liomessage");


            /*
             * ..................................................................................... Level 2 LioContainers Relationship
             */

            // LioDb environment usages:

            g1AdapterService.Uses(lioDbEnvContainer,
                    "CRUD, advise",
                    "local rpc")
                .InteractionStyle = InteractionStyle.Synchronous;

            lioDcg2Container.Uses(lioDbEnvContainer,
                    "CRUD, advising",
                    "local rpc")
                .InteractionStyle = InteractionStyle.Synchronous;

            actionManager.Uses(lioDbEnvContainer,
                    "CRUD",
                    "local rpc")
                .InteractionStyle = InteractionStyle.Synchronous;

            // Technical usages 
            dialogApiEndpoint.Uses(g1AdapterService,
                "Passing requests",
                ".NET Remoting (deprecated)");

            g1AdapterService.Uses(actionManager,
                "Sends messages",
                "liodb-liomessaging");



            /*
             * ..................................................................................... Level 2 LioContainers View
             */
            var containerView = views.CreateContainerView(lioSystem, "Container View",
                "Shows all containers in analytic system");

            containerView.EnableAutomaticLayout();
            containerView.AddAllContainers();
            containerView.AddAnimation(lioDbEnvContainer);
            //containerView.AddNearestNeighbours(biSystem);
            //containerView.AddAnimation(biSystem);


            /*
             * ...................................................................................... DEPLOYMENT Nodes definitions
             */
            var iisHost = model.AddDeploymentNode(
                "IIS 8.5 @G1Server",
                "Hosting local web services",
                "http, http-2, gRPC");

            var windowsG1Host = model.AddDeploymentNode(
                "Windows Server 2016 RC?? x64",
                "On-premise VM at customer side. OS Maintenance: at customers",
                "VMWare | Citrix (??)");

            windowsG1Host.Children.Add(iisHost);

            var osgiFrameComponent01 = model.AddDeploymentNode(
                "Osgi-c01",
                "Osgi framework instance",
                "osgi, openjdk-1.8, activeMQ");

            var osgiFrameComponent02 = model.AddDeploymentNode(
                "Osgi-c02",
                "Osgi framework instance",
                "osgi, openjdk-1.8, activeMQ");

            var lioServiceHost = model.AddDeploymentNode(
                "LioServiceHost @LioFramework.NET",
                ".NET base light hosting with LIO lifetime and messaging support",
                ".NET, WCF");

            var windowsG1Processes = model.AddDeploymentNode(
                "Windows processes Env",
                "LIO processes running in user-mode session",
                "BinUpdate, liobase system lifetime control");

            var windowsG1Daemons = model.AddDeploymentNode(
                "Windows Service Hosting",
                "Lio processes running under Windows lifetime control",
                "Windows build-in, non-s######-service-host, aspcore-3.1");

            windowsG1Host.Children.Add(osgiFrameComponent01);
            windowsG1Host.Children.Add(lioServiceHost);
            windowsG1Host.Children.Add(windowsG1Processes);
            windowsG1Host.Children.Add(windowsG1Daemons);



            var deploymentView = views.CreateDeploymentView(lioSystem, "DeploymentG1", "A deployment diagram showing the live environment.");
            deploymentView.Environment = "Live";
            deploymentView.AddAllDeploymentNodes();









            //var gatewayComponentView = views.CreateComponentView(amwsGateway, "Amws Component View",
            //    "Shows components within AmwsGateway container");

            //gatewayComponentView.AddAllComponents();
            //gatewayComponentView.AddNearestNeighbours(amwsGateway);


            // add some documentation
            StructurizrDocumentationTemplate template = new StructurizrDocumentationTemplate(workspace);
            var lioSystemContext = template.AddContextSection(lioSystem, Format.Markdown,
                "G1 Backend Services - System Context Level 1\n" +
                "\n" +
                "![](embed:SystemContext)");
            var containersContext = template.AddContainersSection(lioSystem, Format.Markdown,
                "Backend applications" + "\n" +
                "![](embed:LioSystem");
            //template.AddComponentsSection(amwsGateway, Format.Markdown, "Amws Gateway component");

            // add some styling
            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle(Tags.Element) { Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Container) { Background = "#08427b", Color = "#ffffff" });
            styles.Add(new ElementStyle(DatabaseTag) { Shape = Shape.Cylinder });

            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            WorkspaceUtils.PrintWorkspaceAsJson(workspace);
            Console.ReadKey();
            structurizrClient.PutWorkspace(WorkspaceId, workspace);

            /*
            // add health checks to the container instances, which return a simple HTTP 200 to say everything is okay
            liveWebApplication.AddHealthCheck("Web Application is running", "https://www.structurizr.com/health");
            liveDatabaseInstance.AddHealthCheck("Database is accessible from Web Application", "https://www.structurizr.com/health/database");

            // the pass/fail status from the health checks is used to supplement any deployment views that include the container instances that have health checks defined
            DeploymentView deploymentView = views.CreateDeploymentView(structurizr, "Deployment", "A deployment diagram showing the live environment.");
            deploymentView.Environment = "Live";
            deploymentView.AddAllDeploymentNodes();

            views.Configuration.Styles.Add(new ElementStyle(Tags.Element) { Color = "#ffffff" });
            views.Configuration.Styles.Add(new ElementStyle(DatabaseTag) { Shape = Shape.Cylinder });

            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            WorkspaceUtils.PrintWorkspaceAsJson(workspace);
            Console.ReadKey();
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
            */


        }
    }
}

