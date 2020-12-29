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
using System.Collections.Generic;

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
        internal const string ICrudTypeTokenName = "ICrudType";

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
        /// Gets the version of bootstrap to be used;
        /// Default <see cref="Bootstrap.Version4"/>
        /// </summary>
        public static Bootstrap BootstrapVersion { get => Options.BootstrapVersion; }

        /// <summary>
        /// Allows adds the services and settings for the use of the package into the client app
        /// </summary>
        /// <param name="services">The application services collection to be use for registered the required services</param>
        /// <param name="optionsFactory">Action to invoke for get the custom settings of the client app</param>
        /// <typeparam name="TDbContext">The type of the database context to be used for interacts with the entities</typeparam>
        /// <returns>The modified application services collection</returns>
        public static IServiceCollection AddGenericCrud<TDbContext>(
            this IServiceCollection services,
            Action<IOptions> optionsFactory = null
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
        /// <summary>
        /// Add the routes of all models related at the cruds
        /// </summary>
        /// <param name="toMapAction">The action to invoke for make the correctly route map</param>
        private static void MapCrudRoutes(Action<Settings.Route, ICrudType> toMapAction)
        {
            var routes = Options.Models
               .ToList()
               .Cast<ICrudTypeRoutable>()
               .SelectMany(ct => Settings.Route.CreateRoutes(ct))
               .ToList();

            routes.ForEach(r =>
            {
                var crudType = r.CrudType as ICrudType;
                if (crudType != null)
                    toMapAction(r, crudType);
            });
        }

#if NETCOREAPP3_1   
        /// <summary>
        /// Add the routes of all models related at the cruds
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the routes to.</param>
        public static void MapCrudRoutes(this IEndpointRouteBuilder endpoints)
        {
            MapCrudRoutes((route, crudType) =>
            {
                endpoints.MapControllerRoute(
                    name: route.Name,
                    pattern: route.Pattern,
                    defaults: new
                    {
                        controller = crudType.ControllerType.Name,
                        action = route.ActionName
                    },
                    constraints: new
                    {
                        modelType = new CrudRouteConstraint(crudType, route.Pattern)
                    },
                    dataTokens: new RouteValueDictionary()
                    {
                        { ModelTypeTokenName, crudType.ModelType },
                        { KeyTokenName, crudType.KeyPropertyName }
                    }
                );
            });
        }
#elif NETCOREAPP2_1
        /// <summary>
        /// Add the routes of all models related at the cruds
        /// </summary>
        /// <param name="routes">the application route collection</param>
        public static void MapCrudRoutes(this IRouteBuilder routes)
        {
            MapCrudRoutes((route, crudType) =>
            {
                routes.MapRoute(
                    name: route.Name,
                    template: route.Pattern,
                    defaults: new
                    {
                        controller = crudType.ControllerType.Name,
                        action = route.ActionName
                    },
                    constraints: new
                    {
                        ModelType = new CrudRouteConstraint(crudType, route.Pattern)
                    },
                    dataTokens: new RouteValueDictionary()
                    {
                        { ModelTypeTokenName, crudType.ModelType },
                        { KeyTokenName, crudType.KeyPropertyName },
                        { ICrudTypeTokenName, crudType }
                    }
                );
            });
        }
#endif
    }
}