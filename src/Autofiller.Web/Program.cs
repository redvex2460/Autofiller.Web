using Autofiller.Data.Models;
using Autofiller.Data.Models.Database;
using Autofiller.Data.Steam;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Autofiller.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Starting Autofiller {Assembly.GetEntryAssembly().GetName().Version} by RedVex2460");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://*:80");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
