using System;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Defines the properties required for generate a CRUD of any model
    /// </summary>
    public class CrudType
    {
        /// <summary>
        /// The type of the model to be used into the CRUD
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// The name of the property used how to key/id of the model
        /// </summary>
        public string KeyPropertyName { get; set; }
        /// <summary>
        /// The custom controller name to be used for the CRUD.
        /// </summary>
        /// <remarks>String empty for used a generic controller</remarks>
        public string ControllerName { get; set; }
        /// <summary>
        /// Generate a new instance for any model
        /// </summary>
        /// <param name="type">The type of the model to be used into the CRUD</param>
        /// <param name="keyPropertyName">The name of the property used how to key/id of the model</param>
        /// <param name="controller">The custom controller name to be used for the CRUD; string empty for used a generic controller</param>
        public CrudType(Type type, string keyPropertyName = "Id", string controller = "")
        {
            Type = type;
            KeyPropertyName = keyPropertyName;
            ControllerName = string.IsNullOrWhiteSpace(controller) ? string.Empty : controller;
        }
    }
}