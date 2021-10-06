using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

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