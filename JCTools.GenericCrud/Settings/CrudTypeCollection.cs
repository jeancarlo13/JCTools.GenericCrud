using System;
using System.Collections.Generic;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Collection of the CRUDs types to be used
    /// </summary>
    public class CrudTypeCollection : List<CrudType>
    {
        /// <summary>
        /// Allows add a new CRUD type
        /// </summary>
        /// <param name="modelType">The type of the model to add</param>
        /// <param name="keyPropertyName">The name of the property used how to key/id of the model</param>
        /// <param name="controller">The custom controller name to be used for the CRUD; string empty for used a generic controller</param>
        public void Add(Type modelType, string keyPropertyName = "Id", string controller = "")
            => Add(new CrudType(modelType, keyPropertyName, controller));        
    }
}