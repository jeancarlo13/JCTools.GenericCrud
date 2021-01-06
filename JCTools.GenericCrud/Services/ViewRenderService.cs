using System;
using System.IO;
using System.Threading.Tasks;
using JCTools.GenericCrud.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace JCTools.GenericCrud.Services
{
    /// <summary>
    /// Defines methods for render the CRUD views
    /// </summary>
    public class ViewRenderService : IViewRenderService
    {
        /// <summary>
        /// The razor engine to be used by render the views
        /// </summary>
        private readonly IRazorViewEngine _viewEngine;

        /// <summary>
        /// <see cref="IModelMetadataProvider"/> instance used to
        /// create <see cref="ModelExplorer"/> instances
        /// </summary>
        private readonly IModelMetadataProvider _metadataProvider;

        /// <summary>
        /// The absolute path to the directory that contains the web-servable
        /// application content files.
        /// </summary>
        private readonly string _webRootPath;

        /// <summary>
        /// The <see cref="ITempDataProvider"/> used to Load and Save data
        /// </summary>
        private readonly ITempDataProvider _tempDataProvider;

        /// <summary>
        /// Instance of <see cref="IServiceProvider" /> used of access 
        /// to the configured services into the startup class
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// <see cref="IHttpContextAccessor"/> instance use of
        /// access to the current HTTP context
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// <see cref="IActionContextAccessor"/> instance used
        /// to located the required views
        /// </summary>
        private readonly IActionContextAccessor _actionContextAccessor;

        /// <summary>
        /// Initialices the service instances
        /// </summary>
        /// <param name="viewEngine">The razor engine to be used by render the views</param>
        /// <param name="httpContextAccessor"><see cref="IHttpContextAccessor"/> instance use of
        /// access to the current HTTP context</param>
        /// <param name="tempDataProvider">The <see cref="ITempDataProvider"/> used to Load 
        /// and Save data</param>
        /// <param name="serviceProvider">Instance of <see cref="IServiceProvider" /> used of access 
        /// to the configured services into the startup class</param>
        /// <param name="metadataProvider"><see cref="IModelMetadataProvider"/> instance used to
        /// create <see cref="ModelExplorer"/> instances</param>
        /// <param name="actionContextAccessor"><see cref="IActionContextAccessor"/> instance used
        /// to located the required views</param>
        /// <param name="environmet"><see cref="IHostingEnvironment"/> instance used to located 
        /// the web root path</param>
        public ViewRenderService(
            IRazorViewEngine viewEngine,
            IHttpContextAccessor httpContextAccessor,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IModelMetadataProvider metadataProvider,
            IActionContextAccessor actionContextAccessor,
#if NETCOREAPP2_1
            IHostingEnvironment environmet
#elif NETCOREAPP3_1 || NET5_0
            Microsoft.AspNetCore.Hosting.IWebHostEnvironment environmet
#endif
        )
        {
            _viewEngine = viewEngine;
            _httpContextAccessor = httpContextAccessor;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _metadataProvider = metadataProvider;
            _actionContextAccessor = actionContextAccessor;
            _webRootPath = environmet.WebRootPath;
        }

        /// <summary>
        /// Allows render a view how to string
        /// </summary>
        /// <typeparam name="TModel">The type of the model to use for rendering the view</typeparam>
        /// <param name="viewName">The name of the view to be rendered</param>
        /// <param name="model">The model to use for rendering the view</param>
        /// <param name="viewData">The data that will be passed to the view for rendering</param>
        /// <returns>The task to be executed</returns>
        public async Task<string> RenderToStringAsync<TModel>(
            string viewName,
            TModel model,
            ViewDataDictionary viewData = null
        )
        {
            var httpContext = _httpContextAccessor.HttpContext
                ?? new DefaultHttpContext { RequestServices = _serviceProvider };

            var viewResult = _viewEngine.FindView(_actionContextAccessor.ActionContext, viewName, false);

            if (viewResult.View == null)
            {
                viewResult = _viewEngine.GetView("~/", viewName, false);

                if (viewResult.View == null)
                    viewResult = _viewEngine.GetView(Path.GetDirectoryName(viewName), Path.GetFileNameWithoutExtension(viewName), false);
            }

            if (viewResult.View == null)
                throw new ArgumentNullException($"{viewName} does not match any available view");

            var viewDictionary = viewData ?? new ViewDataDictionary(_metadataProvider, new ModelStateDictionary());
            viewDictionary.Model = model;

            using (var sw = new StringWriter())
            {
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

        /// <summary>
        ///  Allows render the display view for the specific view
        /// </summary>
        /// <param name="model">The model to use for rendering the view</param>
        /// <param name="htmlHelper"><see cref="IHtmlHelper"/> instance to use for create 
        /// the required <see cref="IHtmlContent"/> instances</param>
        /// <param name="viewData">The data that will be passed to the view for rendering</param>
        /// <param name="propertyName">The name of the related property to the view for rendering</param>
        /// <param name="data">All the entity data to use in rendering the view</param>
        /// <param name="viewPrefix">The prefix of the view to be rendered; "_Details" by default</param>
        /// <returns>The task to be executed</returns>
        public async Task<IHtmlContent> RenderViewFor(
            IViewModel model,
            IHtmlHelper htmlHelper,
            ViewDataDictionary viewData,
            string propertyName,
            object data,
            string viewPrefix = "_Details"
        )
        {
            var viewDictionary = new ViewDataDictionary(_metadataProvider, viewData?.ModelState ?? new ModelStateDictionary())
            {
                Model = data
            };

            foreach (var key in viewData.Keys)
                viewDictionary.Add(key, viewData[key]);

            var file = $"~/Views/{model.GetModelName()}/{viewPrefix}{propertyName}.cshtml";
            var result = htmlHelper.Raw(await RenderToStringAsync(file, data, viewDictionary));
            return result;
        }

        /// <summary>
        /// Allows render the edit view for the specific view
        /// </summary>
        /// <param name="model">The model to use for rendering the view</param>
        /// <param name="htmlHelper"><see cref="IHtmlHelper"/> instance to use for create 
        /// the required <see cref="IHtmlContent"/> instances</param>
        /// <param name="viewData">The data that will be passed to the view for rendering</param>
        /// <param name="propertyName">The name of the related property to the view for rendering</param>
        /// <returns>The task to be executed</returns>
        public Task<IHtmlContent> RenderEditViewFor(
            IEditModel model,
            IHtmlHelper htmlHelper,
            ViewDataDictionary viewData,
            string propertyName
        )
            => RenderViewFor(model, htmlHelper, viewData, propertyName, model.GetData().GetEntity(), "_Edit");

        /// <summary>
        /// Allows rendered the edit view for a CRUD property 
        /// </summary>
        /// <param name="model">The model to use for rendering the view</param>
        /// <param name="htmlHelper"><see cref="IHtmlHelper"/> instance to use for create 
        /// the required <see cref="IHtmlContent"/> instances</param>
        /// <param name="viewData">The data that will be passed to the view for rendering</param>
        /// <param name="propertyName">The name of the related property to the view for rendering</param>
        /// <param name="helperNametoUse">The name of the helper to be invoked for 
        /// the property edition control; "EditorFor" by default</param>
        /// <returns>The task to be executed</returns>
        public async Task<IHtmlContent> CreateEditorFor(
            IViewModel model,
            IHtmlHelper htmlHelper,
            ViewDataDictionary viewData,
            string propertyName,
            string helperNametoUse = "EditorFor")
        {
            var content = $@"
            @model {model.GetModelType().FullName};
            " +
                ((helperNametoUse.ToLower().StartsWith("hidden")) ?
                    $@"<input asp-for=""{propertyName}"" type=""hidden"" />" :
                    $@"@Html.{helperNametoUse}(m => Model.{propertyName}, new {{ htmlAttributes = new {{ @class = ""form-control"" }}}})"
                ) +
                $@"<span asp-validation-for=""{propertyName}"" class=""text-danger""></span>";
            string path = CreateTemporalView(content);

            IHtmlContent result;
            try
            {
                var file = Path.GetFileName(path);
                var viewDictionary = new ViewDataDictionary(_metadataProvider, viewData?.ModelState ?? new ModelStateDictionary())
                {
                    Model = model
                };

                result = htmlHelper.Raw(await RenderToStringAsync($"~/Views/Generic/{file}", model.GetData().GetEntity(), viewDictionary));
            }
            finally
            {
                File.Delete(path);
            }

            return result;
        }

        /// <summary>
        /// Generate a temporary file for render a view
        /// </summary>
        /// <param name="content">The content of the view to be rendered</param>
        /// <returns>The path of the generated file</returns>
        private string CreateTemporalView(string content)
        {
            var path = Path.Combine(
                _webRootPath,
                "..", "Views", "Generic",
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