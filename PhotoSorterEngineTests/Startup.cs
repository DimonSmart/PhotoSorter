using Microsoft.Extensions.DependencyInjection;
using PhotoSorterEngine;


namespace PhotoSorterEngineTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IFileCreationDatetimeExtractor, FileCreationDatetimeExtractor>();
            services.AddTransient<IRenamer, Renamer>();
        }
    }
}