using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SqlServer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = BuildWebHost(args);
            await host.StartAsync();
            SeedData.EnsureSeedData(host.Services);
            await host.StopAsync();
        }

        public static IHost BuildWebHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                        .UseStartup<Startup>()
                        .UseKestrel();
                })
                .Build();
      
    }
}
