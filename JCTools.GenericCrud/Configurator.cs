using System;
using System.Reflection;
using JCTools.GenericCrud.Services;
using JCTools.GenericCrud.Settings;
using JCTools.GenericCrud.Settings.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
#if NETCOREAPP2_1
using Microsoft.AspNetCore.Mvc;
#elif NETCOREAPP3_1 || NET5_0
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
        /// The type of the database context to be use
        /// </summary>
        internal static Type DatabaseContextType;

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

            Options = new Settings.Options();
            optionsFactory?.Invoke(Options);

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddLocalization();
            services.AddTransient<ICrudLocalizer>(services =>
            {
                var logger = services.GetRequiredService<ILoggerFactory>();
                return new CrudLocalizer(Options.ResourceManager, logger);
            });

            var currentAssembly = typeof(Configurator).GetTypeInfo().Assembly;


#if NETCOREAPP2_1
            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.FileProviders.Add(new EmbeddedFileProvider(currentAssembly));
            });

            services
                .AddMvc(o =>
                {
                    o.ModelBinderProviders.Insert(0, new CrudModelBinderProvider());
                })
                .AddRazorOptions(o =>
                {
                    var previous = o.CompilationCallback;
                    o.CompilationCallback = context =>
                    {
                        previous?.Invoke(context);
                        context.Compilation =
                            context.Compilation.AddReferences(MetadataReference.CreateFromFile(currentAssembly.Location));
                    };
                    o.ViewLocationExpanders.Add(new Services.ViewLocationExpander());
                })
                .AddControllersAsServices()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

#elif NETCOREAPP3_1 || NET5_0
            services.Configure<MvcRazorRuntimeCompilationOptions>(o =>
            {
                o.FileProviders.Add(new EmbeddedFileProvider(currentAssembly));
            });

            services.AddRazorPages()
                .AddRazorOptions(o => o.ViewLocationExpanders.Add(new ViewLocationExpander()))
                .AddRazorRuntimeCompilation();

            services
                .AddMvcCore(o => o.ModelBinderProviders.Insert(0, new CrudModelBinderProvider()))
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
#endif

            var policyBuilder = Options.AuthorizationPolicy ?? (b => b.RequireAssertion(c => true));
            services.AddAuthorization(o => o.AddPolicy(Constants.PolicyName, policyBuilder));

            return services;
        }

    }
}