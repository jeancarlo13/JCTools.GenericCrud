using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace JCTools.GenericCrud.Services
{
    /// <summary>
    /// Specifies the contracts for a view location expander that is used by <see cref="RazorViewEngine"/>
    /// instances to determine search paths for a view.
    /// </summary>
    public class ViewLocationExpander : IViewLocationExpander
    {
        /// <summary>
        /// Used to specify the locations that the view engine should search to 
        /// locate views.
        /// </summary>
        /// <param name="context">The <see cref="ViewLocationExpanderContext"/> for the current
        /// view location expansion operation.</param>
        /// <param name="viewLocations">The sequence of view locations to expand.</param>
        /// <returns>A list of expanded view locations.</returns>
        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations
        )
        {
            //{2} is area (view), {1} is controller (Generic), {0} is the action
            string[] locations = new string[] { "/Views/Generic/{0}.cshtml" };
            return viewLocations.Union(locations); //Add mvc default locations after ours
        }

        /// <summary>
        /// Invoked by a <see cref="RazorViewEngine"/> to determine the
        /// values that would be consumed by this instance of <see cref="IViewLocationExpander"/>.
        /// The calculated values are used to determine if the view location has changed
        /// since the last time it was located.
        /// </summary>
        /// <param name="context">The <see cref="ViewLocationExpanderContext"/> for the
        /// current view location expansion operation.</param>
        public void PopulateValues(ViewLocationExpanderContext context)
         => context.Values["customviewlocation"] = nameof(ViewLocationExpander);
    }
}