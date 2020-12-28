using System;
using System.Linq;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Mvc;

namespace JCTools.GenericCrud.Helpers
{
    /// <summary>
    /// Extends the <see cref="IUrlHelper"/> classes 
    /// for generate the correct CRUD actions urls
    /// </summary>
    internal static class CrudUrlsHelper
    {
        /// <summary>
        ///  Generates a URL with an absolute path for the specified CRUD arguments
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/> to be use into the generation.</param>
        /// <param name="actionName">The name of the desired CRUD action </param>
        /// <param name="crudType">The <see cref="ICrudTypeRoutable"/> with the CRUD argument to be used</param>
        /// <param name="scriptName">The name of the script to be used for form the url</param>
        /// <returns>The generated URL.</returns>
        internal static string ScriptRouteUrl(
            this IUrlHelper urlHelper,
            string actionName,
            ICrudTypeRoutable crudType,
            string scriptName
        )
            => GetRouteUrl(urlHelper, actionName, crudType, new { modelType = crudType?.ModelTypeName, fileName = scriptName });

        /// <summary>
        ///  Generates a URL with an absolute path for the specified CRUD arguments
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/> to be use into the generation.</param>
        /// <param name="actionName">The name of the desired CRUD action </param>
        /// <param name="crudType">The <see cref="ICrudTypeRoutable"/> with the CRUD argument to be used</param>
        /// <returns>The generated URL.</returns>
        internal static string RouteUrl(
            this IUrlHelper urlHelper,
            string actionName,
            ICrudTypeRoutable crudType
        )
            => GetRouteUrl(urlHelper, actionName, crudType, new { modelType = crudType?.ModelTypeName });

        /// <summary>
        ///  Generates a URL with an absolute path for the specified CRUD arguments
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/> to be use into the generation.</param>
        /// <param name="actionName">The name of the desired CRUD action </param>
        /// <param name="crudType">The <see cref="ICrudTypeRoutable"/> with the CRUD argument to be used</param>
        /// <param name="id">The id of the entity related to the url</param>
        /// <returns>The generated URL.</returns>
        internal static string RouteUrl<TKey>(
            this IUrlHelper urlHelper,
            string actionName,
            ICrudTypeRoutable crudType,
            TKey id
        )
            => GetRouteUrl(urlHelper, actionName, crudType, new { modelType = crudType?.ModelTypeName, id = id });

        /// <summary>
        ///  Generates a URL with an absolute path for the specified CRUD arguments
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/> to be use into the generation.</param>
        /// <param name="actionName">The name of the desired CRUD action </param>
        /// <param name="crudType">The <see cref="ICrudTypeRoutable"/> with the CRUD argument to be used</param>
        /// <param name="values">Object with the route values to be used</param>
        /// <returns>The generated URL.</returns>
        private static string GetRouteUrl(
            this IUrlHelper urlHelper,
            string actionName,
            ICrudTypeRoutable crudType,
            object values = null
        )
        {
            if (crudType is null)
                throw new System.ArgumentNullException(nameof(crudType));

            if (!crudType.Routes?.Any() ?? true)
                Route.CreateRoutes(crudType);

            var routeName = actionName == Settings.Route.RedirectIndexActionNamePattern
                ? string.Format(actionName, crudType.ModelTypeName)
                : $"{crudType.ModelTypeName}_{actionName}";

            var route = crudType.Routes.FirstOrDefault(r => r.Name == routeName);
            if (route == null)
                throw new InvalidOperationException($"No found route for the action \"{actionName}\"");

            var url = urlHelper.RouteUrl(route.Name, values);
            return url;
        }
    }
}