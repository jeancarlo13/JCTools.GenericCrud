using System;
using System.Collections.Generic;
using JCTools.GenericCrud.Controllers;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Defines the properties required for related a CRUD type with
    /// the mvc routes
    /// </summary>
    internal interface ICrudTypeRoutable
    {
        /// <summary>
        /// The name of the model type related to the CRUD
        /// </summary>
        Type ModelType { get; }

        /// <summary>
        /// Gets or sets the mvc routes
        /// </summary>
        IReadOnlyList<Route> Routes { get; }
    }
}