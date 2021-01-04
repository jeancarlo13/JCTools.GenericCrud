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
using Microsoft.Extensions.Logging;
using System.IO;
#if NETCOREAPP3_1
using Microsoft.AspNetCore.Routing.Matching;
#endif
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;

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
        internal static Settings.Options Options;

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

            Options = new Settings.Options();
            optionsFactory?.Invoke(Options);

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

#if NETCOREAPP2_1
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
            builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, CustomServiceBasedControllerActivator>());
            builder.ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new GenericControllerFeatureProvider(services.BuildServiceProvider()));
                });

#elif NETCOREAPP3_1
            // var defaultEndpointSelector = services.BuildServiceProvider().GetService<EndpointSelector>();
            // services.AddSingleton<EndpointSelector>(provider => new CrudEndpointSelector(defaultEndpointSelector));

            services.Configure<MvcRazorRuntimeCompilationOptions>(o =>
            {
                o.FileProviders.Add(new EmbeddedFileProvider(currentAssembly));
            });

            services.AddRazorPages()
                .AddRazorOptions(o =>
                {
                    o.ViewLocationExpanders.Add(new Services.ViewLocationExpander());
                })
                .AddRazorRuntimeCompilation();

            services.AddMvcCore(o =>
            {
                o.ModelBinderProviders.Insert(0, new Settings.DependencyInjection.CrudModelBinderProvider());
            });

            // services
            //     .AddControllers()
            //     .AddControllersAsServices()
            //     .ConfigureApplicationPartManager(manager =>
            //     {
            //         manager.FeatureProviders
            //             .Add(new GenericControllerFeatureProvider(services.BuildServiceProvider()));
            //     });

            // services.Replace(ServiceDescriptor.Transient<IControllerActivator, CustomServiceBasedControllerActivator>());
#endif

            return services;
        }
        /// <summary>
        /// Add the routes of all models related at the cruds
        /// </summary>
        /// <param name="toMapAction">The action to invoke for make the correctly route map</param>
        private static void MapCrudRoutes(Action<Settings.Route, ICrudType> toMapAction)
        {
            if (toMapAction is null)
                return;

            var cruds = Options.Models.ToList();

            foreach (var crud in cruds)
            {
                if (crud is ICrudTypeRoutable routable)
                {
                    Settings.Route.CreateRoutes(routable)
                        .ToList()
                        .ForEach(r => toMapAction(r, crud));
                }
            }
        }

        // public static IApplicationBuilder UseHttpRequestLogger(this IApplicationBuilder app, ILogger logger)
        // {
        //     app.Use(async (context, next) =>
        //     {
        //         logger.LogInformation($"Header: {JsonConvert.SerializeObject(context.Request.Headers, Formatting.Indented)}");

        //         context.Request.EnableBuffering();
        //         var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        //         logger.LogInformation($"Body: {body}");
        //         context.Request.Body.Position = 0;

        //         logger.LogInformation($"Host: {context.Request.Host.Host}");
        //         logger.LogInformation($"Client IP: {context.Connection.RemoteIpAddress}");
        //         await next?.Invoke();
        //     });

        //     return app;
        // }

        // public static IApplicationBuilder UseCrud(this IApplicationBuilder app)
        // {
        //     Options.Models.AsEnumerable()
        //        .ToList()
        //        .ForEach(ct =>
        //        {
        //            app.Map(
        //                new PathString($"/{ct.ModelType.Name}"),
        //                builder => builder.Use(async (context, next) =>
        //                {
        //                    context.Request.RouteValues.TryAdd("controller", ct.ControllerType.Name);
        //                    context.Request.RouteValues.TryAdd(Configurator.ModelTypeTokenName, ct.ModelType.Name);
        //                    context.Request.RouteValues.TryAdd(Configurator.KeyTokenName, ct.KeyPropertyName);
        //                    context.Request.RouteValues.TryAdd(Configurator.ICrudTypeTokenName, ct);
        //                    await next();
        //                })
        //            );
        //        });

        //     return app;
        // }

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
                    defaults: route.DefaultValues,
                    constraints: new
                    {
                        modelType = new CrudRouteConstraint(route.DefaultValues)
                    }
                ).WithMetadata(new object[]{
                    // new DataAnnotations.CrudConstraintAttibute(),
                    new ControllerAttribute(),
                   // CreateActionDescriptor(route.ActionName, crudType.ControllerType),
                    new DataTokensMetadata(new Dictionary<string, object>())
                });

                // endpoints.MapControllerRoute(
                //     name: "customCrudsDefaultRoute",
                //     pattern: "{controller}/{id}/{action}");
            });
        }

        private static ControllerActionDescriptor CreateActionDescriptor(string methodName, Type controllerType)
        {
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentException($"'{nameof(methodName)}' cannot be null or empty.", nameof(methodName));

            var action = new ControllerActionDescriptor();
            action.SetProperty(new ApiDescriptionActionData());

            if (controllerType == null)
                throw new ArgumentNullException(nameof(controllerType));


            action.MethodInfo = controllerType.GetMethod(methodName)
                ?? controllerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? controllerType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic)
                ?? controllerType.GetMethod($"{methodName}Async", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? controllerType.GetMethod($"{methodName}Async", BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException($"The method {methodName} was not found.");

            action.ControllerTypeInfo = controllerType.GetTypeInfo();
            action.BoundProperties = new List<ParameterDescriptor>();

            foreach (var property in controllerType.GetProperties())
            {
                var bindingInfo = BindingInfo.GetBindingInfo(property.GetCustomAttributes().OfType<object>());
                if (bindingInfo != null)
                {
                    action.BoundProperties.Add(new ParameterDescriptor()
                    {
                        BindingInfo = bindingInfo,
                        Name = property.Name,
                        ParameterType = property.PropertyType,
                    });
                }
            }


            action.Parameters = new List<ParameterDescriptor>();
            foreach (var parameter in action.MethodInfo.GetParameters())
            {
                action.Parameters.Add(new ControllerParameterDescriptor()
                {
                    Name = parameter.Name,
                    ParameterType = parameter.ParameterType,
                    BindingInfo = BindingInfo.GetBindingInfo(parameter.GetCustomAttributes().OfType<object>()),
                    ParameterInfo = parameter
                });
            }

            return action;
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