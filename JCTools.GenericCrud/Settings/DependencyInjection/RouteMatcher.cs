using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.WebUtilities;

namespace JCTools.GenericCrud.Settings.DependencyInjection
{

    internal class RouteMatcher
    {
        public static bool TryMatch(string routeTemplate, string requestPath, IQueryCollection query, out RouteValueDictionary values)
        {
            // The TemplateParser can only parse the route part, and not the query string.
            // If the template provided by the user also has a query string, we separate that and match it manually.
            var regex = new Regex(@"(.*)(\?[^{}]*$)");
            var match = regex.Match(routeTemplate);
            if (match.Success)
            {
                var queryString = match.Groups[2].Value;
                routeTemplate = match.Groups[1].Value;

                var queryInTemplate = QueryHelpers.ParseQuery(queryString);

                if (!query.All(arg => queryInTemplate.ContainsKey(arg.Key.TrimStart('?')) && queryInTemplate[arg.Key.TrimStart('?')] == arg.Value))
                {
                    values = null;
                    return false;
                }
            }

            var template = TemplateParser.Parse(routeTemplate);

            var matcher = new TemplateMatcher(template, GetDefaults(template));

            values = new RouteValueDictionary();

            return matcher.TryMatch(requestPath, values);
        }

        private static RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();

            foreach (var parameter in parsedTemplate.Parameters)
            {
                if (parameter.DefaultValue != null)
                {
                    result.Add(parameter.Name, parameter.DefaultValue);
                }
            }

            return result;
        }
    }

}
