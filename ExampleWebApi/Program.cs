using ExampleWebApi.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ExampleWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .SetupDevelopmentDatabase()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
