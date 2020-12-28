using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JCTools.GenericCrud.Settings.DependencyInjection
{
    /// <summary>
    /// Defines the contract that a class must implement in order to check whether a
    /// URL parameter value is valid for a constraint.
    /// </summary>
    internal class CrudRouteConstraint : IRouteConstraint
    {
        /// <summary>
        /// The crud type of the related model to the route
        /// </summary>
        private ICrudType _crudType;
        /// <summary>
        /// The template that define the route
        /// </summary>
        private string _template;
        /// <summary>
        /// Initializes the current instance
        /// </summary>
        /// <param name="crudType">The CRUD type of the related model to the route</param>
        /// <param name="template">The template that define the route</param>
        internal CrudRouteConstraint(ICrudType crudType, string template)
        {
            _crudType = crudType;
            _template = template;
        }
        /// <summary>
        /// Determines whether the URL parameter contains a valid value for this constraint.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <param name="route">The router that this constraint belongs to.</param>
        /// <param name="routeKey">The name of the parameter that is being checked.</param>
        /// <param name="values">A dictionary that contains the parameters for the URL.</param>
        /// <param name="routeDirection">An object that indicates whether the constraint check is being performed when
        /// an incoming request is being handled or when a URL is being generated.</param>
        /// <returns>True if the URL parameter contains a valid value; otherwise, false.</returns>
        public bool Match(
            HttpContext httpContext,
            IRouter route,
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection
        )
        {
            var isMatch = values[routeKey]?.ToString().ToLowerInvariant().Equals(_crudType.ModelType.Name.ToLowerInvariant()) ?? false;
            if (isMatch && !values.Keys.Contains(Configurator.ICrudTypeTokenName))
                values[Configurator.ICrudTypeTokenName] = _crudType;

            return isMatch;
        }

    }
}