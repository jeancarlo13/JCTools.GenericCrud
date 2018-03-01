using System.Threading.Tasks;
using JCTools.GenericCrud.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JCTools.GenericCrud.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName);
        Task<string> RenderToStringAsync<TModel>(string viewName, TModel model);
        string RenderToString<TModel>(string viewPath, TModel model);
        string RenderToString(string viewPath);
        System.Threading.Tasks.Task<IHtmlContent> CreateEditorForAsync(
            ICrudDetails model,
            IHtmlHelper htmlHelper,
            string propertyName,
            string helperNametoUse = "EditorFor"
        );
    }
}