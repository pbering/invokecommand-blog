﻿using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Blog.Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseAzureAppServices()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}