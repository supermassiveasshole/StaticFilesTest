using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Routing;
using System.IO;
using StaticFilesTest.Data;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace StaticFilesTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /* services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });*/


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddRouting();
            services.AddSession();
            //services.AddDbContext<AddmissionContext>(options =>options.UseSqlServer("Server=.;Database=Admission;Trusted_Connection=True;MultipleActiveResultSets=true"));
            //(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<AddmissionContext>(options =>options.UseMySql("Server=localhost;Database=ef;User=root;Password=password;",
                mySqlOptions =>
                {
                    mySqlOptions.ServerVersion(new Version(10, 1, 37), ServerType.MariaDb); // replace with your Server Version and Type
                }
            ));
            services.AddScoped<StaticFilesTest.Services.DeliverFiles>();
            services.AddScoped<StaticFilesTest.Services.GetAdmissionList>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseSession();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "/api/{controller=Home}/{action=Index}/{id?}");
            });
            var trackPackageRouteHandler = new RouteHandler(context =>
            {
                var routeValues = context.Request.Path;
                var file = Directory.GetCurrentDirectory()+"/wwwroot"+routeValues;
                var fileContent=string.Empty;
                try
                {
                    var reader=new StreamReader(file);
                    fileContent=reader.ReadToEnd();
                }
                catch(Exception ex)
                {
                    var reader=new StreamReader(Directory.GetCurrentDirectory()+"/wwwroot"+"/index.html");
                    fileContent=reader.ReadToEnd();
                }
                context.Response.StatusCode=200;
                context.Response.ContentType="text/html";
                return context.Response.WriteAsync(fileContent);
            });
            var routeBuilder = new RouteBuilder(app, trackPackageRouteHandler);
            routeBuilder.MapRoute("Track Package Route","{*slug}");
            var route=routeBuilder.Build();
            app.UseRouter(route);
        }
    }
}
