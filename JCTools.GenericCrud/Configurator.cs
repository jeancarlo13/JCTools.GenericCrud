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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

namespace JCTools.GenericCrud
{
    public static class Configurator
    {
        internal const string ModelTypeTokenName = "ModelType";
        internal const string KeyTokenName = "ModelKey";
        internal static Options Options;
        public static IServiceCollection ConfigureGenericCrud(this IServiceCollection services, Action<Options> options = null)
        {
            var currentAssembly = typeof(Configurator).GetTypeInfo().Assembly;
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IActionSelector, CrudActionSelector>();

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
            options?.Invoke(Options);

            builder.ConfigureApplicationPartManager(p =>
                p.FeatureProviders.Add(new GenericControllerFeatureProvider())
            );

            return services;
        }
        /// <summary>
        /// Add the routes of all models relateed at the cruds
        /// </summary>
        /// <param name="routes">the application route collection</param>
        public static void MapCrudRoutes(this IRouteBuilder routes)
        {
            foreach (var item in Options.Models)
            {
                var dataTokens = new RouteValueDictionary()
                {
                    {
                    ModelTypeTokenName,
                    item.Type
                    },
                    {
                    KeyTokenName,
                    item.KeyPropertyName
                    }
                };

                routes.MapRoute(item.Type, "Details", "{model}/{id}/details", dataTokens);
                routes.MapRoute(item.Type, "Delete", "{model}/{id}/delete", dataTokens);
                routes.MapRoute(item.Type, "DeleteConfirm", "{model}/{id}/deleteconfirm", dataTokens);
                routes.MapRoute(item.Type, "Create", "{model}/create", dataTokens);
                routes.MapRoute(item.Type, "Save", "{model}/Save", dataTokens);
                routes.MapRoute(item.Type, "Edit", "{model}/{id}/edit", dataTokens);
                routes.MapRoute(item.Type, "SaveChangesAsync", "{model}/SaveChanges/{id}", dataTokens);
                routes.MapRoute(item.Type, "GetScript", "{model}/{filename}.js", dataTokens);
                routes.MapRoute(item.Type, "Index", "{model}", dataTokens);
                routes.MapRoute(item.Type, $"{item.Type.Name}_FullIndex", "Index2", "{model}/{id}/index", dataTokens);

            }
        }
        /// <summary>
        /// Add a new route into the routes collections
        /// </summary>
        /// <param name="routes">Collection of routes of the app</param>
        /// <param name="type">The model type to map</param>
        /// <param name="action">the action name to map </param>
        /// <param name="tokens">The tokes to add at the route</param>
        /// <param name="template">The string with the template of the route</param>
        private static void MapRoute(
            this IRouteBuilder routes,
            Type type,
            string action,
            string template,
            RouteValueDictionary tokens)
            => MapRoute(routes, type, $"{type.Name}_{action}", action, template, tokens);
        /// <summary>
        /// Add a new route into the routes collections
        /// </summary>
        /// <param name="routes">Collection of routes of the app</param>
        /// <param name="type">The model type to map</param>
        /// <param name="action">the action name to map </param>
        /// <param name="tokens">The tokes to add at the route</param>
        /// <param name="template">The string with the template of the route</param>
        private static void MapRoute(
            this IRouteBuilder routes,
            Type type,
            string routeName,
            string action,
            string template,
            RouteValueDictionary tokens
        )
        {
            routes.MapRoute(
                name: routeName,
                template : template,
                defaults : new
                {
                    controller = "GenericController`3",
                        action = action
                },
                constraints : new
                {
                    model = new Settings.CrudRouteConstraint(type, template)
                },
                dataTokens : tokens
            );
        }
    }
}