using System;
using JCTools.GenericCrud.Controllers;
using Microsoft.EntityFrameworkCore;

namespace JCTools.GenericCrud.Settings
{

    /// <summary>
    /// Defines the properties required for generate a CRUD of any model
    /// </summary>
    /// <typeparam name="TModel">The type of the model to be used into the CRUD</typeparam>
    /// <typeparam name="TCustomController">The custom controller type to be used for the CRUD</typeparam>
    /// <typeparam name="TKey">The type of the property identifier of the entity model</typeparam>
    /// <typeparam name="TContext">The type of the database context to be used by get/stored the entities</typeparam>
    internal class CrudType<TModel, TKey, TCustomController, TContext> : CrudType<TModel>
        where TModel : class, new()
        where TContext : DbContext
        where TCustomController : GenericController<TContext, TModel, TKey>

    {
        /// <summary>
        /// The controller to be used for entry to the CRUD actions
        /// </summary>
        /// <remarks>The default controller is <see cref="GenericController{TContext, TModel, TKey}"/></remarks>
        public override Type ControllerType { get => typeof(TCustomController); }

        /// <summary>
        /// Generate a new instance for any model
        /// </summary>
        /// <param name="keyPropertyName">The name of the property used how to key/id of the model</param>
        public CrudType(string keyPropertyName = "Id")
            : base(keyPropertyName)
        {
            UseGenericController = false;
        }
    }

    /// <summary>
    /// Defines the properties required for generate a CRUD of any model
    /// </summary>
    /// <typeparam name="TModel">The type of the model to be used into the CRUD</typeparam>
    internal class CrudType<TModel> : ICrudType
        where TModel : class, new()

    {
        /// <summary>
        /// The type of the model to be used into the CRUD
        /// </summary>
        public virtual Type ModelType { get => typeof(TModel); }

        /// <summary>
        /// The name of the property used how to key/id of the model
        /// </summary>
        public virtual string KeyPropertyName { get; }

        /// <summary>
        /// The type of the property used how to key/id of the model
        /// </summary>
        public Type KeyPropertyType { get; }

        /// <summary>
        /// The controller to be used for entry to the CRUD actions
        /// </summary>
        /// <remarks>The default controller is <see cref="GenericController{TContext, TModel, TKey}"/></remarks>
        public virtual Type ControllerType { get; }

        /// <summary>
        /// True if the controller to be used into the current represented CRUD 
        /// is <see cref="GenericController{TContext, TModel, TKey}"/>; Another, false
        /// </summary>
        public bool UseGenericController { get; protected set; }

        /// <summary>
        /// Generate a new instance for any model
        /// </summary>
        /// <param name="keyPropertyName">The name of the property used how to key/id of the model</param>
        public CrudType(string keyPropertyName = "Id")
        {
            KeyPropertyName = keyPropertyName;
            KeyPropertyType = ModelType.GetProperty(KeyPropertyName)?.PropertyType
                ?? throw new InvalidOperationException($"The \"{KeyPropertyName}\" is not found in the model \"{ModelType.FullName}\"");

            ControllerType = Configurator.GenericControllerType.MakeGenericType(
                Configurator.DatabaseContextType,
                ModelType,
                KeyPropertyType
            );

            UseGenericController = true;
        }

        /// <summary>
        /// Actives a new controller instance for attend the HTTP request
        /// </summary>
        /// <param name="serviceProvider">Instance of <see cref="IServiceProvider" /> used 
        /// of access to the configured services into the startup class</param>
        /// <returns>The generated instance</returns>
        public IGenericController GetControllerInstance(IServiceProvider serviceProvider)
            => Activator.CreateInstance(ControllerType, serviceProvider, KeyPropertyName) as IGenericController;
    }
}