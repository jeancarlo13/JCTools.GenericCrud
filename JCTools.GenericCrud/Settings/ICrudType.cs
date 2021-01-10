using System;
using System.Collections.Generic;
using JCTools.GenericCrud.Controllers;
using JCTools.GenericCrud.Models;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Defines the properties required for generate a CRUD of any model
    /// </summary>
    public interface ICrudType
    {
        /// <summary>
        /// The type of the model to be used into the CRUD
        /// </summary>
        Type ModelType { get; }

        /// <summary>
        /// The name of the property used how to key/id of the model
        /// </summary>
        string KeyPropertyName { get; }

        /// <summary>
        /// The type of the property used how to key/id of the model
        /// </summary>
        Type KeyPropertyType { get; }

        /// <summary>
        /// True indicates that the user can edit the value of the Id / Key property 
        /// and can overwrite its value; False (default) other case
        /// </summary>
        bool KeyPropertyIsEditable { get; }

        /// <summary>
        /// The controller to be used for entry to the CRUD actions
        /// </summary>
        /// <remarks>The default controller is <see cref="GenericController"/></remarks>
        Type ControllerType { get; }

        /// <summary>
        /// True if the controller to be used into the current represented CRUD 
        /// is <see cref="GenericController"/>; Another, false
        /// </summary>
        bool UseGenericController { get; }

        /// <summary>
        /// Allows get the properties that should appear into the CRUD views
        /// </summary>
        /// <param name="includeNoVisibleColumns">True for include the not visible columns; 
        /// False for return only the visible columns</param>
        /// <param name="localizer">The instance of <see cref="IStringLocalizer"/> used for translate 
        /// the texts to displayed into the view</param>
        /// <returns>The found properties</returns>
        IEnumerable<PropertyData> GetProperties(IStringLocalizer localizer, bool includeNoVisibleColumns = false);


        /// <summary>
        /// Allows get the Key/Id property value of the specific instance
        /// </summary>
        /// <param name="obj">The instance to be evaluated</param>
        /// <returns>The found Key/Id property value or null</returns>
        object GetKeyPropertyValue(object obj);
    }
}