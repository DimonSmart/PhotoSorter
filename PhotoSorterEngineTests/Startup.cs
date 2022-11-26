using Microsoft.Extensions.DependencyInjection;
using PhotoSorterEngine;
using PhotoSorterEngine.Interfaces;

namespace PhotoSorterEngineTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IFileCreationDatetimeExtractor, FileCreationDatetimeExtractor>();
            services.AddTransient<IRenamer, Renamer>();
            services.AddSingleton<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>();
        }
    }
}