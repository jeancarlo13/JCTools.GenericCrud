using System;
using System.IO;
using System.Threading.Tasks;
using JCTools.GenericCrud.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

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
        private readonly IHostingEnvironment _enviroment;

        public ViewRenderService(IRazorViewEngine viewEngine, IHttpContextAccessor httpContextAccessor,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IModelMetadataProvider metadataProvider,
            IActionContextAccessor actionContextAccessor,
            IHostingEnvironment env)
        {
            _viewEngine = viewEngine;
            _httpContextAccessor = httpContextAccessor;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _metadataProvider = metadataProvider;
            _actionContextAccessor = actionContextAccessor;
            _enviroment = env;

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

                if (viewResult.View == null)
                {
                    viewResult = _viewEngine.GetView("~/", viewName, false);

                    if (viewResult.View == null)
                        viewResult = _viewEngine.GetView(Path.GetDirectoryName(viewName), Path.GetFileNameWithoutExtension(viewName), false);
                }

                if (viewResult.View == null)
                    throw new ArgumentNullException($"{viewName} does not match any available view");

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

        public async System.Threading.Tasks.Task<IHtmlContent> CreateEditorForAsync(
            IBaseDetails model, 
            IHtmlHelper htmlHelper, 
            string propertyName,
            string helperNametoUse = "EditorFor")
        {

            var content = $@"
            @model {model.GetGenericType().FullName};
            @Html.{helperNametoUse}(m => Model.{propertyName}, new {{ htmlAttributes = new {{ @class = ""form-control"" }}}})
            <span asp-validation-for=""{propertyName}"" class=""text-danger""></span>
            ";
            string path = CreateTemporalView(content);

            IHtmlContent result;
            try
            {
                var file = Path.GetFileName(path);
                result = htmlHelper.Raw(await RenderToStringAsync($"~/Views/Generic/{file}", model.GetData()));
            }
            finally
            {
                File.Delete(path);
            }

            return result;
        }

        private string CreateTemporalView(string content)
        {
            var path = Path.Combine(
                            _enviroment.WebRootPath,
                            "..",
                            "Views",
                            "Generic",
                            $"{Path.GetFileNameWithoutExtension(Path.GetTempFileName())}.cshtml"
                        );

            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, content);
            return path;
        }
    }
}