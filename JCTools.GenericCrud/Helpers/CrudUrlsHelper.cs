using System;
using System.Linq;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Mvc;
using JCTools.GenericCrud.Models;
using Microsoft.AspNetCore.Routing;

namespace JCTools.GenericCrud.Helpers
{
    /// <summary>
    /// Add extensor methods for the generation of urls and redirections 
    /// from the CRUD actions
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
            => GetRouteUrl(urlHelper, actionName, crudType, fileName: scriptName);

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
            => GetRouteUrl(urlHelper, actionName, crudType);

        /// <summary>
        ///  Generates a URL with an absolute path for the specified CRUD arguments
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/> to be use into the generation.</param>
        /// <param name="actionName">The name of the desired CRUD action </param>
        /// <param name="crudType">The <see cref="ICrudTypeRoutable"/> with the CRUD argument to be used</param>
        /// <param name="id">The id of the entity related to the url</param>
        /// <param name="message">The message to be displayed to the user</param>
        /// <returns>The generated URL.</returns>
        internal static string RouteUrl<TKey>(
            this IUrlHelper urlHelper,
            string actionName,
            ICrudTypeRoutable crudType,
            TKey id,
            IndexMessages message = IndexMessages.None
        )
            => GetRouteUrl(urlHelper, actionName, crudType, id, message: message);

        /// <summary>
        ///  Generates a URL with an absolute path for the specified CRUD arguments
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/> to be use into the generation.</param>
        /// <param name="actionName">The name of the desired CRUD action </param>
        /// <param name="crudType">The <see cref="ICrudTypeRoutable"/> with the CRUD argument to be used</param>
        /// <param name="fileName">The name of the script to be used for form the url</param>
        /// <param name="id">The id of the entity related to the url</param>
        /// <param name="message">The message to be displayed to the user</param>
        /// <returns>The generated URL.</returns>
        private static string GetRouteUrl(
            this IUrlHelper urlHelper,
            string actionName,
            ICrudTypeRoutable crudType,
            object id = null,
            string fileName = null,
            IndexMessages message = IndexMessages.None
        )
        {
            var route = GetRoute(actionName, crudType, throwIfNoFoundRoute: false);

            var values = new RouteValueDictionary();
            values.Add(nameof(RouteDefaultValues.Controller), route.DefaultValues.Controller);
            values.Add(nameof(RouteDefaultValues.Action), route.DefaultValues.Action);
            values.Add(nameof(RouteDefaultValues.ModelType), route.DefaultValues.ModelType);

            if (id != null)
                values.Add(nameof(id), id);
            if (!string.IsNullOrWhiteSpace(fileName))
                values.Add(nameof(fileName), fileName);
            if (message != IndexMessages.None)
                values.Add(nameof(message), message);

            var url = urlHelper.RouteUrl(route?.Name ?? actionName, values);

            if (string.IsNullOrWhiteSpace(url))
            {
                url = route.Pattern;
                if (id != null)
                {
                    var str = id.ToString();
                    if (string.IsNullOrWhiteSpace(str))
                        url = url.Replace("/{id}", str);
                    else
                        url = url.Replace("{id}", str);
                }
                if (!string.IsNullOrWhiteSpace(fileName))
                    url = url.Replace("{filename}", fileName);
                if (message != IndexMessages.None)
                    url = url.Replace("{message}", message.ToString());

                if (!url.StartsWith("/"))
                    url = $"/{url}";
            }

            return url;
        }

        /// <summary>
        /// Gets the correct route from the specified arguments 
        /// </summary>
        /// <param name="actionName">The name of the desired CRUD action </param>
        /// <param name="crudType">The <see cref="ICrudTypeRoutable"/> with the CRUD argument to be used</param>
        /// <param name="throwIfNoFoundRoute">True for generate a exception if not found a route; Another, false.</param>
        /// <returns>The found route</returns>
        private static Settings.Route GetRoute(string actionName, ICrudTypeRoutable crudType, bool throwIfNoFoundRoute = true)
        {
            if (crudType is null)
                throw new System.ArgumentNullException(nameof(crudType));

            if (!crudType.Routes?.Any() ?? true)
                Settings.Route.CreateRoutes(crudType);

            var routeName = actionName == Settings.Route.RedirectIndexActionNamePattern
                ? string.Format(actionName, crudType.ModelTypeName)
                : $"{crudType.ModelTypeName}_{actionName}";

            var route = crudType.Routes.FirstOrDefault(r => r.Name == routeName);
            if (route == null && throwIfNoFoundRoute)
                throw new InvalidOperationException($"No found route for the action \"{actionName}\"");

            return route;
        }


    }
}