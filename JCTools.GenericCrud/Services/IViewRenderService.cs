using System.Threading.Tasks;
using JCTools.GenericCrud.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace JCTools.GenericCrud.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync<TModel>(string viewName, TModel model, ViewDataDictionary viewData = null);
        System.Threading.Tasks.Task<IHtmlContent> CreateEditorFor(
            IBaseDetails model,
            IHtmlHelper htmlHelper,
            ViewDataDictionary viewData,
            string propertyName,
            string helperNametoUse = "EditorFor"
        );
        System.Threading.Tasks.Task<IHtmlContent> RenderViewFor(
            IBaseDetails model,
            IHtmlHelper htmlHelper,
            ViewDataDictionary viewData,
            string propertyName
        );
        System.Threading.Tasks.Task<IHtmlContent> RenderViewFor(
            IBase model,
            IHtmlHelper htmlHelper,
            ViewDataDictionary viewData,
            string propertyName,
            object data);
    }
}