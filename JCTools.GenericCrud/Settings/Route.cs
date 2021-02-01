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
        /// The action name to map with the route
        /// </summary>
        private string _actionName;

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
        internal const string SaveChangesActionName = "SaveChanges";

        /// <summary>
        /// The name of the Crud action used of get access to the js script 
        /// required for use Modals in the CRUD views 
        /// </summary>
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
        /// The URL pattern of the route.
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// The name of the route.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The default values of the route
        /// </summary>
        public RouteDefaultValues DefaultValues { get; }

        /// <summary>
        /// Init an instance with the specified settings
        /// </summary>
        /// <param name="crudType">The related <see cref="ICrudTypeRoutable"/> to the new route.</param>
        /// <param name="actionName">The name of the related action to the new route.</param>
        /// <param name="pattern">The pattern to be used in the created urls from the new routes.
        /// <para>If is null is used the pattern: 
        /// {<paramref name="crudType"/>.ModelType.Name}/{Id}/{<paramref name="actionName"/>}</para>
        /// </param>
        /// <param name="routeName">The name of the new route; null for use the 
        /// <paramref name="actionName"/> parameter</param>
        internal Route(
            ICrudTypeRoutable crudType,
            string actionName,
            string pattern = null,
            string routeName = null
        )
        {
            _actionName = actionName;

            Pattern = pattern
                ?? string.Format($"{{{{{Constants.ModelTypeTokenName}}}}}/{{{{id}}}}/{{0}}", actionName.ToLowerInvariant());

            Name = string.IsNullOrWhiteSpace(routeName)
                ? $"{crudType.ModelType.Name}_{_actionName}"
                : routeName;

            var type = crudType as ICrudType;
            DefaultValues = new RouteDefaultValues
            {
                Controller = type?.ControllerType.Name,
                Action = _actionName,
                ModelType = type?.ModelType.Name
            };
        }

        /// <summary>
        /// Returns the string representation of the class
        /// </summary>
        public override string ToString()
            => $"{Name}, {_actionName}, {Pattern}";

    }
}