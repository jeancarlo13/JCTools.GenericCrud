using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JCTools.GenericCrud.Settings
{
    public class CrudRouteConstraint : IRouteConstraint
    {
        private string _modelType;
        public CrudRouteConstraint(Type modelType)
        {
            _modelType = modelType.Name.ToLowerInvariant();
        }
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return values[routeKey].ToString().ToLowerInvariant().Equals(_modelType);
        }
    }
}