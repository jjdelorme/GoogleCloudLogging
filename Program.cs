using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Google.Cloud.Diagnostics.AspNetCore;

namespace Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string projectId = "jasondel-test-project";

            using IHost host = CreateHostBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddGoogleExceptionLogging(options => {
                        options.ProjectId = projectId;
                        options.ServiceName = "MyLogger";
                        options.Version = "V1";
                    });
                })
                .Build();
            
            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddProvider(GoogleLoggerProvider.Create(
                    serviceProvider: null, 
                    projectId));
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();
            IExceptionLogger exceptionLogger = 
                host.Services.GetRequiredService<IExceptionLogger>();

            int i = 0;
            System.Timers.Timer t = new System.Timers.Timer();
            t.Elapsed += (o, a) => {
                try 
                {
                    FooBar(logger, i++);
                }
                catch (Exception e)
                {
                    exceptionLogger.Log(e);
                }
            };

            t.Interval = 1000 * 5;
            t.Enabled = true;
            
            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args);

        static void FooBar(ILogger logger, int i)
        {
            string caller = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (i % 5 == 0)
            {
                logger.LogError("Something happened in {operation} on {i}!", caller, i);
                
            }
            else if (i % 7 == 0)
            {
                throw new ArgumentException("We don't like this agrument.");
            }
            else 
            {
                logger.LogInformation("{operation} result is {result}", caller, i);
            }
        }
    }
}
