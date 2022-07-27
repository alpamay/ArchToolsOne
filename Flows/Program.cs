using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using NLog;

namespace Flows
{
    public class Program
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public static int Main(string[] args)
        {
            var rootCommand = new RootCommand(description: "Welcome to Flows! Running executable: flows.exe");
            
            rootCommand.AddOption(new Option(
                new[] { "--verbose", "-v" },
                @"Verbose output switch")
            {
                Argument = new Argument<bool>(() => false),
            });

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

            rootCommand.AddCommand(ParseExtraLogFile());
            return rootCommand.Invoke(args);


            // TODO: Move it to command "serve"
            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static Command ParseExtraLogFile()
        {
            var command = new Command(
                "parse",
                "Parse extralog files looking for EventSourcing flows. Will find only first in log!");

            command.AddOption(
                new Option(
                    new[] {"--source-file", "-f"},
                    @"File containing Participations Dependencies and Participations Event flow logs")
                {
                    Argument = new Argument<string>(),
                });

            command.Handler = CommandHandler.Create<string>((sourceFile) =>
            {
                Log.Info(@"...");

                try
                {
                    var text = File.OpenText(sourceFile);
                    var sourceFileGenerator = new StateDiagramsGenerator();

                    sourceFileGenerator.ParseForMnemonics(text);
                    //var puml = sourceFileGenerator.GetPumlCodeFullText();
                    var puml = sourceFileGenerator.GetPumlCodeFullTextNodesVersion();


                    if (File.Exists(sourceFile + ".puml"))
                        File.Delete(sourceFile + ".puml");
                    
                    File.WriteAllText(sourceFile + ".puml", puml);

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
