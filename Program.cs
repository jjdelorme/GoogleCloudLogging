﻿using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            int i = 0;
            System.Timers.Timer t = new System.Timers.Timer();
            t.Elapsed += (o, e) => {
                FooBar(logger, i++);
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
                logger.LogError("Something happened on {i}!");
            }
            else 
            {
                logger.LogInformation("{operation} result is {result}", caller, i);
            }
        }
    }
}
