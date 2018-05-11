using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using JCTools.GenericCrud;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Test
{
    public class Startup
    {
        private readonly IHostingEnvironment _enviroment;

        public Startup(IConfiguration configuration, IHostingEnvironment enviroment)
        {
            Configuration = configuration;
            _enviroment = enviroment;
        }

        public IConfiguration Configuration
        {
            get;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var resourcesPath = "../../../Resources";
            services.AddJsonLocalization(o =>
            {
                o.ResourcesPath = resourcesPath;
            });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new []
                {
                new CultureInfo("en-US"),
                new CultureInfo("es-MX")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.ConfigureGenericCrud(o =>
            {
                o.UseModals = true;
                o.ContextCreator = () => new Test.Data.Context();
                o.Models.Add(typeof(Models.Country));
                o.Models.Add(typeof(Models.Genre), nameof(Models.Genre.Name));
            });

            services.AddMvc()
                .AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts =>
                    {
                        opts.ResourcesPath = resourcesPath;
                    })
                .AddDataAnnotationsLocalization();

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
            }

            app.UseStaticFiles();

            app.UseRequestLocalization();

            app.UseMvc(routes =>
            {
                routes.MapCrudRoutes();
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}