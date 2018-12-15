using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StaticFilesTest.Data;

namespace StaticFilesTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host=CreateWebHostBuilder(args).Build();
            using(var scope=host.Services.CreateScope())
            {
                var services=scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AddmissionContext>();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
            /* var file=Directory.GetCurrentDirectory()+"/hello.xlsx";
            byte[] bytes;
            string fileContent;
            using(var reader=new StreamReader(file))
            {
                fileContent=reader.ReadToEnd();
                bytes=System.Text.Encoding.Default.GetBytes(fileContent);
                fileContent=Convert.ToBase64String(bytes);
            }
            fileContent="data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,"+fileContent;*/
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
