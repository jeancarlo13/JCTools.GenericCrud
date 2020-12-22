using System;


namespace JCTools.GenericCrud.Helpers
{
    /// <summary>
    /// Provides extensors methods for <see cref="IServiceProvider" /> instances
    /// /// </summary>
    public static class ServiceProviderExtensors
    {
        /// <summary>
        /// Create the instance of the GenericController according the specific arguments
        /// </summary>
        /// <param name="serviceProvider">The application service provider to be use use for the controller creation process</param>
        /// <param name="model">The model type to be use</param>
        /// <param name="key">The Key/Id property name of the model</param>
        /// <returns>The created instance </returns>
        public static object CreateGenericController(this IServiceProvider serviceProvider, Type model, string key)
        {
            var genericControllerType = CreateGenericControllerType(serviceProvider, model, key);

            var controller = Activator.CreateInstance(
                genericControllerType,
                new object[] { serviceProvider, key }
            );

            return controller;
        }

        /// <summary>
        /// Create dinamically the type of the CenericController with the specific model type 
        /// </summary>
        /// <param name="serviceProvider">The application service provider to be use use for the controller creation process</param>
        /// <param name="model">The model type to use</param>
        /// <param name="key">the key property name of the model</param>
        /// <returns>The created type</returns>
        public static Type CreateGenericControllerType(this IServiceProvider serviceProvider, Type model, string key)
        {
            var keyType = model.GetProperty(key)?.PropertyType ??
                throw new ArgumentOutOfRangeException(nameof(key));

            return Configurator.GenericControllerType
                .MakeGenericType(Configurator.DatabaseContextType, model, keyType);
        }

    }
}