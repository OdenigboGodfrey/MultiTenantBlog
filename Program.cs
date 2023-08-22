using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MultiTenantBlogTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // webBuilder.UseStartup<Startup>();
                    
                    // doing this locally to be able to run the application on the system ip i.e 0.0.0.0
                    // so i can use custom subdomains locally
                    webBuilder.UseStartup<Startup>().UseUrls(new string [] {"http://0.0.0.0:5000", "https://0.0.0.0:5001"});
                });
    }
}
