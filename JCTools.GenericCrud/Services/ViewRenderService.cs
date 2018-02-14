using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace JCTools.GenericCrud.Services
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModelMetadataProvider _metadataProvider;
        private readonly IActionContextAccessor _actionContextAccessor;

        public ViewRenderService(IRazorViewEngine viewEngine, IHttpContextAccessor httpContextAccessor,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IModelMetadataProvider metadataProvider,
            IActionContextAccessor actionContextAccessor)
        {
            _viewEngine = viewEngine;
            _httpContextAccessor = httpContextAccessor;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _metadataProvider = metadataProvider;
            _actionContextAccessor = actionContextAccessor;

        }

        public string RenderToString<TModel>(string viewPath, TModel model)
        {
            try
            {
                var viewEngineResult = _viewEngine.GetView("~/", viewPath, false);

                if (!viewEngineResult.Success)
                {
                    throw new InvalidOperationException($"Couldn't find view {viewPath}");
                }

                var view = viewEngineResult.View;

                using(var sw = new StringWriter())
                {
                    var viewContext = new ViewContext()
                    {
                    HttpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext
                    {
                    RequestServices = _serviceProvider
                    },
                    ViewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                    Model = model
                    },
                    Writer = sw
                    };
                    view.RenderAsync(viewContext).GetAwaiter().GetResult();
                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error ending email.", ex);
            }
        }

        public async Task<string> RenderToStringAsync<TModel>(string viewName, TModel model)
        {
            var httpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };
            
            using(var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(_actionContextAccessor.ActionContext, viewName, false);
                // Fallback - the above seems to consistently return null when using the EmbeddedFileProvider
                if (viewResult.View == null)
                {
                    viewResult = _viewEngine.GetView("~/", viewName, false);
                }

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(_metadataProvider, new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    _actionContextAccessor.ActionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(_actionContextAccessor.ActionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

        public string RenderToString(string viewPath)
        {
            return RenderToString(viewPath, string.Empty);
        }

        public Task<string> RenderToStringAsync(string viewName)
        {
            return RenderToStringAsync<string>(viewName, string.Empty);
        }
    }
}