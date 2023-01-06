using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhotoSorterEngine.Interfaces;
using PhotoSorterEngine;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace PhotoSorter.CLI
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            await Host.CreateDefaultBuilder(args)
            .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
            .ConfigureServices((hostContext, services) =>
            {
                services
                   .AddHostedService<ConsoleHostedService>()
                   .AddSingleton<IFileEnumerator, FileEnumerator>()
                   .AddSingleton<IFileCreationDatetimeExtractor, FileCreationDatetimeExtractor>()
                   .AddSingleton<IRenamer, Renamer>()
                   .AddSingleton<IFileReorderCalculator, FileReorderCalculator>()
                   .AddSingleton<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>()
                   .AddOptions<PhotoSorterSettings>()
                       .Bind(hostContext.Configuration.GetSection("PhotoSorter"));

            })
             .ConfigureHostConfiguration(hostConfig =>
             {
                 hostConfig.AddEnvironmentVariables();
                 hostConfig.AddCommandLine(args);
             })
             .RunConsoleAsync();
        }
    }
}