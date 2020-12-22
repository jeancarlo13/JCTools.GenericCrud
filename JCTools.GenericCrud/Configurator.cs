using System;
using System.Reflection;
using JCTools.GenericCrud.Services;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IActionSelector, CrudActionSelector>();

            var currentAssembly = typeof(Configurator).GetTypeInfo().Assembly;
            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.FileProviders.Add(new EmbeddedFileProvider(currentAssembly));
            });

            var builder = services.AddMvc();
            builder.AddRazorOptions(o =>
            {
                var previous = o.CompilationCallback;
                o.CompilationCallback = context =>
                {
                    previous?.Invoke(context);
                    context.Compilation =
                        context.Compilation.AddReferences(MetadataReference.CreateFromFile(currentAssembly.Location));
                };
                o.ViewLocationExpanders.Add(new Services.ViewLocationExpander());
            });
            builder.AddControllersAsServices();

            builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, Settings.CustomServiceBasedControllerActivator>());

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
        /// <param name="routes">the application route collection</param>
        public static void MapCrudRoutes(this IRouteBuilder routes)
        {
            var cruds = Options.Models.GetEnumerator();
            while (cruds.MoveNext())
            {
                var item = cruds.Current;
                var dataTokens = new RouteValueDictionary()
                {
                    { ModelTypeTokenName, item.Type },
                    { KeyTokenName, item.KeyPropertyName }
                };

                var controller = string.IsNullOrWhiteSpace(item.ControllerName) ? GenericControllerType.Name : item.ControllerName;

                routes.MapRoute(item.Type, "Details", controller, "{model}/{id}/details", dataTokens);
                routes.MapRoute(item.Type, "Delete", controller, "{model}/{id}/delete", dataTokens);
                routes.MapRoute(item.Type, "DeleteConfirm", controller, "{model}/{id}/deleteconfirm", dataTokens);
                routes.MapRoute(item.Type, "Create", controller, "{model}/create", dataTokens);
                routes.MapRoute(item.Type, "Save", controller, "{model}/Save", dataTokens);
                routes.MapRoute(item.Type, "Edit", controller, "{model}/{id}/edit", dataTokens);
                routes.MapRoute(item.Type, "SaveChangesAsync", controller, "{model}/SaveChanges/{id}", dataTokens);
                routes.MapRoute(item.Type, "GetScript", controller, "{model}/{filename}.js", dataTokens);
                routes.MapRoute(item.Type, "Index", controller, "{model}", dataTokens);
                routes.MapRoute(item.Type, $"{item.Type.Name}_RedirectedIndex", "Index", controller, "{model}/{id}/index", dataTokens);

            }
        }
        /// <summary>
        /// Add a new route into the routes collections
        /// </summary>
        /// <param name="routes">Collection of routes of the app</param>
        /// <param name="type">The model type to map</param>
        /// <param name="action">the action name to map </param>
        /// <param name="controllerName">The name of the controller to be received the requests</param>
        /// <param name="template">The string with the template of the route</param>
        /// <param name="tokens">The tokes to add at the route</param>
        private static void MapRoute(
            this IRouteBuilder routes,
            Type type,
            string action,
            string controllerName,
            string template,
            RouteValueDictionary tokens)
            => MapRoute(routes, type, $"{type.Name}_{action}", action, controllerName, template, tokens);
        /// <summary>
        /// Add a new route into the routes collections
        /// </summary>
        /// <param name="routes">Collection of routes of the app</param>
        /// <param name="type">The model type to map</param>
        /// <param name="routeName">The name of the route to be generated</param>
        /// <param name="action">the action name to map </param>
        /// <param name="controllerName">The name of the controller to be received the requests</param>
        /// <param name="template">The string with the template of the route</param>
        /// <param name="tokens">The tokes to add at the route</param>
        private static void MapRoute(
            this IRouteBuilder routes,
            Type type,
            string routeName,
            string action,
            string controllerName,
            string template,
            RouteValueDictionary tokens
        )
        {
            routes.MapRoute(
                name: routeName,
                template: template,
                defaults: new
                {
                    controller = controllerName,
                    action = action
                },
                constraints: new
                {
                    model = new Settings.CrudRouteConstraint(type, template)
                },
                dataTokens: tokens
            );
        }
    }
}