using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JCTools.GenericCrud;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing.Matching;

namespace Test3._1
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
            services.AddDbContext<Data.Context>(builder =>
                builder.UseSqlite("Data Source=../Data/MoviesGallery.db")
            );

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddGenericCrud<Data.Context>(o =>
           {
               o.UseModals = true;
               o.Models.Add<Models.Country>();
               o.Models.Add<Models.Genre>(nameof(Models.Genre.Name));
               o.Models.Add<Models.Movie, int, Controllers.MovieController, Data.Context>();
           });

            services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            EndpointSelector selector
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            var supportedCultures = new[] { "en-US", "es-MX" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            // app.UseHttpRequestLogger(loggerFactory.CreateLogger<Startup>());
            // app.UseCrud();

            var logger = loggerFactory.CreateLogger<Startup>();

            var routes = actionDescriptorCollectionProvider.ActionDescriptors.Items
                        .Cast<ControllerActionDescriptor>()
                        .Select(x => new
                        {
                            Action = x.ActionName,
                            Controller = x.ControllerName,
                            Name = x.AttributeRouteInfo?.Name,
                            DisplayName = x.DisplayName,
                            Template = x.AttributeRouteInfo?.Template,
                            Method = x.MethodInfo.ToString(),
                            RouteValues = x.RouteValues,
                            Properties = x.Properties,
                            ActionConstraints = x.ActionConstraints,
                            Parameters = x.Parameters.Select(p => p.Name),
                            BoundProperties = x.BoundProperties.Select(p => p.Name),
                            FilterDescriptors = x.FilterDescriptors
                        })
                        // .Where(x => string.IsNullOrWhiteSpace(controllerName) || x.Controller.Equals(controllerName))
                        .ToList();

            logger.LogDebug($"Routes: {Newtonsoft.Json.JsonConvert.SerializeObject(routes)}");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapCrudRoutes();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            routes = actionDescriptorCollectionProvider.ActionDescriptors.Items
                        .Cast<ControllerActionDescriptor>()
                        .Select(x => new
                        {
                            Action = x.ActionName,
                            Controller = x.ControllerName,
                            Name = x.AttributeRouteInfo?.Name,
                            DisplayName = x.DisplayName,
                            Template = x.AttributeRouteInfo?.Template,
                            Method = x.MethodInfo.ToString(),
                            RouteValues = x.RouteValues,
                            Properties = x.Properties,
                            ActionConstraints = x.ActionConstraints,
                            Parameters = x.Parameters.Select(p => p.Name),
                            BoundProperties = x.BoundProperties.Select(p => p.Name),
                            FilterDescriptors = x.FilterDescriptors
                        })
                        // .Where(x => string.IsNullOrWhiteSpace(controllerName) || x.Controller.Equals(controllerName))
                        .ToList();

            logger.LogDebug($"Routes: {Newtonsoft.Json.JsonConvert.SerializeObject(routes)}");
        }
    }
}
