using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JCTools.GenericCrud.Helpers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Allows configured the required controllers for generate the configured CRUDs 
    /// </summary>
    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        /// <summary>
        /// The application services provider to be use for the controller creation process
        /// </summary>
        private IServiceProvider _serviceProvider;
        /// <summary>
        /// Initializes the current instance
        /// </summary>
        /// <param name="serviceProvider">The application services provider to be use for the controller creation process</param>
        public GenericControllerFeatureProvider(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        /// <summary>
        /// Register the <see cref="Controllers.GenericController{TContext, TModel, TKey}"/> into the application controller features
        /// </summary>
        /// <param name="parts">The list of <see cref="ApplicationPart"/> of the application.</param>
        /// <param name="feature">The feature instance to populate.</param>
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var item in Configurator.Options.Models.ToList())
            {
                var genericControllerType = ServiceProviderExtensors.CreateGenericControllerType(_serviceProvider, item.Type, item.KeyPropertyName);
                feature.Controllers.Add(genericControllerType.GetTypeInfo());
            }
        }
    }
}