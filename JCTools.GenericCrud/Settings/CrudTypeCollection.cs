using System;
using System.Collections.Generic;

namespace JCTools.GenericCrud.Settings
{
    public class CrudTypeCollection : List<CrudType>
    {
        public void Add(Type modelType, string keyPropertyName = "Id", string controller = "")
         => Add(new CrudType(modelType, keyPropertyName, controller));
    }
}