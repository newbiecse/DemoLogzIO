using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;

namespace elastic_kibana
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((context, builder) =>
               {
                   var env = context.HostingEnvironment;

                   string envFile = $"appsettings.{env.EnvironmentName}.json";

                   Console.WriteLine($"envFile: {envFile}");

                   builder
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                   if (env.IsDevelopment())
                   {
                       var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                       if (appAssembly != null)
                       {
                           builder.AddUserSecrets(appAssembly, optional: true);
                       }
                   }

                   builder.AddEnvironmentVariables();

                   if (args != null)
                   {
                       builder.AddCommandLine(args);
                   }
               })
               .UseStartup<Startup>()
               .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
               .ReadFrom.Configuration(hostingContext.Configuration)
               .Enrich.FromLogContext()
               .WriteTo.LogzIo("QcsBKjPtMWUEWriInZrAdotsmxISodZm", "MY_JSON_LOG_TYPE", useHttps: true)
               )
               .Build();
    }
}
