using System;
using JCTools.GenericCrud.Controllers;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Defines the properties required for generate a CRUD of any model
    /// </summary>
    internal interface ICrudType
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
        /// The controller to be used for entry to the CRUD actions
        /// </summary>
        /// <remarks>The default controller is <see cref="GenericController{TContext, TModel, TKey}"/></remarks>
        Type ControllerType { get; }

        /// <summary>
        /// True if the controller to be used into the current represented CRUD 
        /// is <see cref="GenericController{TContext, TModel, TKey}"/>; Another, false
        /// </summary>
        bool UseGenericController { get; }

        /// <summary>
        /// Actives a new controller instance for attend the HTTP request
        /// </summary>
        /// <param name="serviceProvider">Instance of <see cref="IServiceProvider" /> used 
        /// of access to the configured services into the startup class</param>
        /// <returns>The generated instance</returns>
        IGenericController GetControllerInstance(IServiceProvider serviceProvider);
    }
}