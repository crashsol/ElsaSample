using System;
using System.Threading.Tasks;
using Elsa.Activities.Console.Extensions;
using Elsa.Activities.Timers.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace demo5
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(loggin => loggin.AddConsole())
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                await host.StartAsync();
                await host.WaitForShutdownAsync();
            }

        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddElsa()
                .AddConsoleActivities()
                .AddWorkflow<RecurringWorkflow>()
                .AddTimerActivities(options => options.Configure(x => x.SweepInterval = Duration.FromSeconds(1)));
        }
    }
}
