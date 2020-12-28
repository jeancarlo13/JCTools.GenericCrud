using System.Collections.Generic;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Represents a mvc route to use by to map into the client app the CRUD controller
    /// </summary>
    internal class Route
    {
        /// <summary>
        /// The pattern that include an entity id/key that will be used to make the path pattern
        /// </summary>
        private static readonly string _defaultIdPattern = $"{{{{{Configurator.ModelTypeTokenName}}}}}/{{{{id}}}}/{{0}}";
        /// <summary>
        /// The pattern that not include an entity id/key that will be used to make the path pattern
        /// </summary>
        private static readonly string _defaultPattern = $"{{{{{Configurator.ModelTypeTokenName}}}}}/{{0}}";

        /// <summary>
        /// Contains the settings of the related CRUD with the route
        /// </summary>
        public ICrudType CrudType { get; }
        /// <summary>
        /// The URL pattern of the route.
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// The action name to map with the route
        /// </summary>
        public string ActionName { get; }

        /// <summary>
        /// /// The name of the route.
        /// </summary>
        public string Name { get; }

        public Route(
            ICrudType crudType,
            string actionName,
            string pattern = null,
            string routeName = null
        )
        {
            CrudType = crudType;
            ActionName = actionName;
            Pattern = string.Format(pattern ?? _defaultIdPattern, actionName.ToLowerInvariant());
            Name = string.IsNullOrWhiteSpace(routeName)
                ? $"{CrudType.ModelType.Name}_{ActionName}"
                : routeName;
        }

        public static IReadOnlyList<Route> CreateRoutes(ICrudType crudType)
        {
            if (crudType is null)
                throw new System.ArgumentNullException(nameof(crudType));

            return new List<Route>()
            {
                new Route(crudType, "Details"),
                new Route(crudType, "Delete"),
                new Route(crudType, "DeleteConfirm"),
                new Route(crudType, "Create", _defaultPattern),
                new Route(crudType, "Save", _defaultPattern),
                new Route(crudType, "Edit"),
                new Route(crudType, "SaveChangesAsync", pattern: $"{{{{{Configurator.ModelTypeTokenName}}}}}/SaveChanges/{{{{id}}}}"),
                new Route(crudType, "GetScript", pattern: $"{{{{{Configurator.ModelTypeTokenName}}}}}/{{{{filename}}}}.js"),
                new Route(crudType, "Index", pattern: $"{{{{{Configurator.ModelTypeTokenName}}}}}"),
                new Route(crudType, "Index", routeName: $"{crudType.ModelType.Name}_RedirectedIndex")
            };
        }
    }
}