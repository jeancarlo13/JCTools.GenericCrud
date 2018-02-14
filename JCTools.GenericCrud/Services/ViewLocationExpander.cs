using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace JCTools.GenericCrud.Services
{
    public class ViewLocationExpander: IViewLocationExpander {

    /// <summary>
    /// Used to specify the locations that the view engine should search to 
    /// locate views.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="viewLocations"></param>
    /// <returns></returns>
    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations) {
        //{2} is area, {1} is controller,{0} is the action
        string[] locations = new string[] { "/Views/Generic/{0}.cshtml"};
        return viewLocations.Union(locations);          //Add mvc default locations after ours
    }


    public void PopulateValues(ViewLocationExpanderContext context) {
        context.Values["customviewlocation"] = nameof(ViewLocationExpander);
    }
}
}