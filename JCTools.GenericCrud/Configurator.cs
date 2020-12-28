using System;
using System.Linq;
using System.Reflection;
using JCTools.GenericCrud.Services;
using JCTools.GenericCrud.Settings;
using JCTools.GenericCrud.Settings.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

#if NETCOREAPP3_1
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
#endif
namespace JCTools.GenericCrud
{
    /// <summary>
    /// Provides methods for initialize, registered and configure the package in the client app
    /// </summary>
    public static class Configurator
    {
        /// <summary>
        /// The name of the <see cref="Controllers.GenericController{TContext, TModel, TKey}" /> type
        /// </summary>
        internal static readonly Type GenericControllerType = typeof(Controllers.GenericController<,,>);

        /// <summary>
        /// The type of the database context to be use
        /// </summary>
        internal static Type DatabaseContextType;

        /// <summary>
        /// The name of the token with the model type of a CRUD
        /// </summary>
        internal const string ModelTypeTokenName = "ModelType";
        /// <summary>
        /// The name of the token with the Id/Key property name to be use into a CRUD 
        /// </summary>
        internal const string KeyTokenName = "ModelKey";
        /// <summary>
        /// The configured settings for all CRUDs
        /// </summary>
        internal static Options Options;

        /// <summary>
        /// Allows adds the services and settings for the use of the package into the client app
        /// </summary>
        /// <param name="services">The application services collection to be use for registered the required services</param>
        /// <param name="optionsFactory">Action to invoke for get the custom settings of the client app</param>
        /// <typeparam name="TDbContext">The type of the database context to be used for interacts with the entities</typeparam>
        /// <returns>The modified application services collection</returns>
        public static IServiceCollection AddGenericCrud<TDbContext>(
            this IServiceCollection services,
            Action<Options> optionsFactory = null
        )
            where TDbContext : DbContext
        {
            DatabaseContextType = typeof(TDbContext);

            var currentAssembly = typeof(Configurator).GetTypeInfo().Assembly;

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();


#if NETCOREAPP2_1
            services.AddSingleton<IActionSelector, CrudActionSelector>();

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.FileProviders.Add(new EmbeddedFileProvider(currentAssembly));
            });
#elif NETCOREAPP3_1
            services.Configure<MvcRazorRuntimeCompilationOptions>(o =>
            {
                o.FileProviders.Add(new EmbeddedFileProvider(currentAssembly));
            });

            services.AddRazorPages().AddRazorRuntimeCompilation();
#endif

            var builder = services.AddMvc();
            builder.AddRazorOptions(o =>
            {
#if NETCOREAPP2_1
                var previous = o.CompilationCallback;
                o.CompilationCallback = context =>
                {
                    previous?.Invoke(context);
                    context.Compilation =
                        context.Compilation.AddReferences(MetadataReference.CreateFromFile(currentAssembly.Location));
                };
#endif
                o.ViewLocationExpanders.Add(new Services.ViewLocationExpander());
            });
            builder.AddControllersAsServices();

            builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, CustomServiceBasedControllerActivator>());

            Options = new Options();
            optionsFactory?.Invoke(Options);

            builder.ConfigureApplicationPartManager(p =>
                p.FeatureProviders.Add(
                    new GenericControllerFeatureProvider(services.BuildServiceProvider())
                )
            );

            return services;
        }

#if NETCOREAPP3_1
        public static void UseGenericCrud(this IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                var endpointFeature = context.Features[typeof(IEndpointFeature)] as IEndpointFeature;
                var endpoint = endpointFeature?.Endpoint;

                //note: endpoint will be null, if there was no
                //route match found for the request by the endpoint route resolver middleware
                if (endpoint != null)
                {
                    var routePattern = (endpoint as RouteEndpoint)?.RoutePattern
                                                                ?.RawText;

                    Console.WriteLine("Name: " + endpoint.DisplayName);
                    Console.WriteLine($"Route Pattern: {routePattern}");
                    Console.WriteLine("Metadata Types: " + string.Join(", ", endpoint.Metadata));
                }
                return next();
            });
        }

        /// <summary>
        /// Add the routes of all models related at the cruds
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the routes to.</param>
        public static void MapCrudRoutes(this IEndpointRouteBuilder endpoints)
        {
            var routes = Options.Models
                .ToList()
                .SelectMany(Settings.Route.CreateRoutes)
                .ToList();

            routes.ForEach(r => endpoints.MapControllerRoute(
                    name: r.Name,
                    pattern: r.Pattern,
                    defaults: new
                    {
                        controller = r.CrudType.ControllerType.Name,
                        action = r.ActionName
                    },
                    constraints: new
                    {
                        modelType = new CrudRouteConstraint(r.CrudType.ModelType, r.Pattern)
                    },
                    dataTokens: new RouteValueDictionary()
                    {
                        { ModelTypeTokenName, r.CrudType.ModelType },
                        { KeyTokenName, r.CrudType.KeyPropertyName },
                        { "ICrudType", r.CrudType}
                    }
                ));
        }
#elif NETCOREAPP2_1
        /// <summary>
        /// Add the routes of all models related at the cruds
        /// </summary>
        /// <param name="routes">the application route collection</param>
        public static void MapCrudRoutes(this IRouteBuilder routes)
        {
            Options.Models
                .ToList()
                .SelectMany(Settings.Route.CreateRoutes)
                .ToList()
                .ForEach(r => routes.MapRoute(
                    name: r.Name,
                    template: r.Pattern,
                    defaults: new
                    {
                        controller = r.CrudType.ControllerType.Name,
                        action = r.ActionName
                    },
                    constraints: new
                    {
                        ModelType = new CrudRouteConstraint(r.CrudType.ModelType, r.Pattern)
                    },
                    dataTokens: new RouteValueDictionary()
                    {
                        { ModelTypeTokenName, r.CrudType.ModelType },
                        { KeyTokenName, r.CrudType.KeyPropertyName }
                    }
                ));
        }
#endif
    }
}