using System.Threading.Tasks;
using JCTools.GenericCrud.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace JCTools.GenericCrud.Services
{
    /// <summary>
    /// Defines methods for render the CRUD views
    /// </summary>
    public interface IViewRenderService
    {
        /// <summary>
        /// Allows render a view how to string
        /// </summary>
        /// <typeparam name="TModel">The type of the model to use for rendering the view</typeparam>
        /// <param name="viewName">The name of the view to be rendered</param>
        /// <param name="model">The model to use for rendering the view</param>
        /// <param name="viewData">The data that will be passed to the view for rendering</param>
        /// <returns>The task to be executed</returns>
        Task<string> RenderToStringAsync<TModel>(
            string viewName,
            TModel model,
            ViewDataDictionary viewData = null);

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

        Task<IHtmlContent> CreateEditorFor(
            IViewModel model,
            IHtmlHelper htmlHelper,
            ViewDataDictionary viewData,
            string propertyName,
            string helperNametoUse = "EditorFor"
        );

        /// <summary>
        /// Allows render the edit view for the specific view
        /// </summary>
        /// <param name="model">The model to use for rendering the view</param>
        /// <param name="htmlHelper"><see cref="IHtmlHelper"/> instance to use for create 
        /// the required <see cref="IHtmlContent"/> instances</param>
        /// <param name="viewData">The data that will be passed to the view for rendering</param>
        /// <param name="propertyName">The name of the related property to the view for rendering</param>
        /// <returns>The task to be executed</returns>
        Task<IHtmlContent> RenderEditViewFor(
            IEditModel model,
            IHtmlHelper htmlHelper,
            ViewDataDictionary viewData,
            string propertyName
        );

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
        Task<IHtmlContent> RenderViewFor(
            IViewModel model,
            IHtmlHelper htmlHelper,
            ViewDataDictionary viewData,
            string propertyName,
            object data,
            string viewPrefix = "_Details");
    }
}