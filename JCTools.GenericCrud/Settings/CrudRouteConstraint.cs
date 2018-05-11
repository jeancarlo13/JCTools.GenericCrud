using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JCTools.GenericCrud.Settings
{
    public class CrudRouteConstraint : IRouteConstraint
    {
        private string _modelType;
        private string _template;
        public CrudRouteConstraint(Type modelType, string template)
        {
            _modelType = modelType.Name.ToLowerInvariant();
            _template = template;
        }
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return values[routeKey].ToString().ToLowerInvariant().Equals(_modelType);
        }
    }
}