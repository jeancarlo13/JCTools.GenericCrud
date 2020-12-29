using System.Collections.Generic;
using System.Linq;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Represents a mvc route to use by to map into the client app the CRUD controller
    /// </summary>
    internal class Route
    {
        /// <summary>
        /// The name of the CRUD see entity details action
        /// </summary>
        internal const string DetailsActionName = "Details";

        /// <summary>
        /// The name of the CRUD action for see the details of the entity to delete
        /// </summary>        
        internal const string DeleteActionName = "Delete";

        /// <summary>
        /// The name of the CRUD action for the confirmation of the delete entity
        /// </summary>                
        internal const string DeleteConfirmActionName = "DeleteConfirm";

        /// <summary>
        /// The name of the CRUD action for the entity creation
        /// </summary>   
        internal const string CreateActionName = "Create";

        /// <summary>
        /// The name of the CRUD action used for save the new entities
        /// </summary>
        internal const string SaveActionName = "Save";

        /// <summary>
        /// The name of the CRUD action used for see the entity edit form 
        /// </summary>
        internal const string EditActionName = "Edit";

        /// <summary>
        /// The name of the CRUD action used for store the entities changes
        /// </summary>
        internal const string SaveChangesActionName = "SaveChangesAsync";

        /// <summary>
        /// The name of the Crud action used of get access to the js script 
        /// required for use Modals in the CRUD views 
        /// /// </summary>
        internal const string GetScriptActionName = "GetScript";

        /// <summary>
        /// The name of the CRUD action used for get access to the view of all entities
        /// </summary>
        internal const string IndexActionName = "Index";

        /// <summary>
        /// The CRUD action name pattern to access the view of all entities
        /// by highlighting a particular entity
        /// </summary>
        internal const string RedirectIndexActionNamePattern = "{0}_RedirectedIndex";

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
        public ICrudTypeRoutable CrudType { get; }
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

        internal Route(
            ICrudTypeRoutable crudType,
            string actionName,
            string pattern = null,
            string routeName = null
        )
        {
            CrudType = crudType;
            ActionName = actionName;
            Pattern = string.Format(pattern ?? _defaultIdPattern, actionName.ToLowerInvariant());
            Name = string.IsNullOrWhiteSpace(routeName)
                ? $"{CrudType.ModelTypeName}_{ActionName}"
                : routeName;
        }

        /// <summary>
        /// Generate the mvc routes collection for the specified CRUD type 
        /// </summary>
        /// <param name="crudType">The related crud type to the routes to be generated</param>
        /// <returns>The generated routes collection</returns>
        internal static IReadOnlyList<Route> CreateRoutes(ICrudTypeRoutable crudType)
        {
            if (crudType is null)
                throw new System.ArgumentNullException(nameof(crudType));

            if (!crudType.Routes?.Any() ?? true)
                crudType.Routes = new List<Route>()
                {
                    new Route(crudType, DetailsActionName),
                    new Route(crudType, DeleteActionName),
                    new Route(crudType, DeleteConfirmActionName),
                    new Route(crudType, CreateActionName, _defaultPattern),
                    new Route(crudType, SaveActionName, _defaultPattern),
                    new Route(crudType, EditActionName),
                    new Route(crudType, SaveChangesActionName, pattern: $"{{{{{Configurator.ModelTypeTokenName}}}}}/{{{{id}}}}/SaveChanges"),
                    new Route(crudType, GetScriptActionName, pattern: $"{{{{{Configurator.ModelTypeTokenName}}}}}/{{{{filename}}}}.js"),
                    new Route(crudType, IndexActionName, pattern: $"{{{{{Configurator.ModelTypeTokenName}}}}}"),
                    new Route(crudType, IndexActionName, routeName: string.Format(RedirectIndexActionNamePattern, crudType.ModelTypeName))
                };

            return crudType.Routes;
        }

    }
}