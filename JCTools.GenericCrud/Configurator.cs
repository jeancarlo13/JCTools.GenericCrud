using System;
using System.Reflection;
using JCTools.GenericCrud.Services;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

namespace JCTools.GenericCrud
{
    public static class Configurator
    {
        internal static Options Options;
        public static IServiceCollection ConfigureGenericCrud(this IServiceCollection services, Action<Options> options = null)
        {
            var currentAssembly = typeof(Configurator).GetTypeInfo().Assembly;
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.FileProviders.Add(new EmbeddedFileProvider(currentAssembly));
            });

            services.AddMvc()
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
                });

            Options = new Options();
            options?.Invoke(Options);

            return services;
        }

    }
}