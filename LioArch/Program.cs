using System;
using System.Collections.Generic;
using LioArch;
using Structurizr;
using Structurizr.Api;
using Structurizr.Documentation;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text.Json;
using LioArch.Builders;
using LioArch.Container;
using LioArch.Modeling;
using LioArch.Persistence;
using MongoDB.Bson;
using NLog;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace LioArch
{
    internal class Program
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public static int Main(params string[] args)
        {

            var rootCommand = new RootCommand(
                description: "LioArch project - Welcome! Running executable: lioarch");

            rootCommand.AddOption(CliParameters.VerboseOption());

            rootCommand.Handler = CommandHandler.Create<bool>(
                (verbose) =>
                {
                    try
                    {
                        Log.Info(rootCommand.Description);
                        Log.Info("Specify some command to do more. Use [command] --help for details");

                    }
                    catch (Exception anyException)
                    {
                        Log.Fatal($"Breaking problem. Exception: {anyException.Message}\n{anyException.StackTrace}");
                    }
                });

            rootCommand.AddCommand(ReloadDatabase());
            rootCommand.AddCommand(StructurirzLioArch());
            rootCommand.AddCommand(StructurirzLioArchIntegrationLayer());

            return rootCommand.Invoke(args);
        }

        private static Command StructurirzLioArchIntegrationLayer()
        {
            var command = new Command(
                "uploadStep1",
                "Used to upload prototype builders");

            command.AddOption(new Option(
                "--workspace",
                "Structurirz workspace id [default:0]")
            {
                Argument = new Argument<int>(() => 0)
            });
           
            command.Handler = CommandHandler.Create<int>((workspace) =>
            {
                Log.Info($"Let LioArch visualizes! Uploading to Structurirz - workplaceId={workspace}");

                try
                {
                    var builder = new StructurizrWorkspaceBuilder(new WorkspaceRepository());
                    builder.BuildAndUpload(workspace);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            });

            return command;
        }

        private static Command StructurirzLioArch()
        {
            var command = new Command(
                "upload",
                "Used to upload given model from Arch DB to Structurizr");

            command.AddOption(new Option(
                "--workspace",
                "Structurirz workspace id [default:0]")
            {
                Argument = new Argument<int>(() => 0)
            });
            command.AddOption(new Option(
                "--system",
                "Specifies system id from software systems repository, default: \"LioSystem\"")
            {
                Argument = new Argument<string>(() => "LioSystem")
            });

            command.Handler = CommandHandler.Create<int, string>((workspace, system) =>
            {
                Log.Info($"Let LioArch visualize! Uploading to Structurirz - workplaceId={workspace}");

                try
                {
                    var mongoClient = new MongoClient("mongodb://localhost:27017");

                    var modelDbInstanceName = ModelLoadingSourceRepo.Get(system)?.DbName;

                    if (modelDbInstanceName == null)
                    {
                        Log.Error($"Requested systemId={system} cannot be found in modeling sources. Shutting down.");
                        return;
                    }

                    var dbInstance = mongoClient.GetDatabase(modelDbInstanceName);

                    BsonClassMapper.MapToBson();

                    //var builder = new SoftwareSystemBuilder(new WorkspaceRepository(), dbInstance);

                    var builder = new LioSystemBuilder();

                    //builder.StructurizePersistedArchModel(workspace, system);
                    builder.StructurizePersistedArchModel(workspace);

                    // Next step training area! Be careful, but comment what is disturbing.

                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            });

            return command;
        }


        private static Command ReloadDatabase()
        {
            var command = new Command(
                "reload",
                "Reloading Arch DB with generated content from Json repository");

            //command.AddOption(new Option(
            //    "--database",
            //    "Database name (default: lioarch")
            //{
            //    Argument = new Argument<string>(() => "lioarch")
            //});

            command.AddOption(new Option(
                "--model",
                "Model source name. Available: LioSystem, LbslSystem, Lio-DAC | default= Lio-DAC")
                {
                    Argument = new Argument<string>(() => "Lio-DAC")
                }
            );

            command.Handler = CommandHandler.Create<string>((model) =>
            {
                Log.Info(@"Reloading local mongo with JSON repo content...");

                try
                {

                    BsonClassMapper.MapToBson();
                    SoftwareSystemRepository softwareRepository;
                    
                    var mongoClient = new MongoClient("mongodb://localhost:27017");

                    var modelSource = ModelLoadingSourceRepo.Get(model);

                    if( modelSource == null)
                    {
                        throw new ArgumentException($"System id not found: {model}. Cannot continue!");
                    }
                    
                    softwareRepository = new SoftwareSystemRepository(mongoClient, modelSource);

                    softwareRepository.EnsureCreated();
                    softwareRepository.ReloadSoftwareSystems();
                    softwareRepository.ReloadContainers();
                    softwareRepository.ReloadComponents();
                    softwareRepository.ReloadDeploymentNodes();
                    softwareRepository.ReloadPersonas();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            });

            return command;
        }
    }
}

