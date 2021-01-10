namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Defines the constants to be used into the package
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The name of the arguments into the routes for get the entity name
        /// related to the CRUD
        /// </summary>
        internal const string EntitySettingsRouteKey = "entitySettings";

        /// <summary>
        /// The name of the arguments into the routes for get the entity model value
        /// related to the CRUD
        /// </summary>
        internal const string EntityModelRouteKey = "entityModel";

        /// <summary>
        /// The name of the token with the Id/Key property name to be use into a CRUD 
        /// </summary>
        internal const string KeyTokenName = "ModelKey";
        
        /// <summary>
        /// The name of the token with the model type of a CRUD
        /// </summary>
        internal const string ModelTypeTokenName = "ModelType";
    }
}