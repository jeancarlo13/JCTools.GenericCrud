using System;
using System.Collections.Generic;

namespace JCTools.GenericCrud.Settings
{
    public class CrudTypeCollection : List<CrudType>
    {
        public void Add(Type modelType, string keyPropertyName = "Id") => Add(new CrudType(modelType, keyPropertyName));
    }
}