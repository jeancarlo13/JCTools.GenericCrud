using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace JCTools.GenericCrud.Settings.DependencyInjection
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
            var cruds = Configurator.Options.Models.ToList();
                // .ToList(item => item.UseGenericController);

            foreach (var crud in cruds)
                feature.Controllers.Add(crud.ControllerType.GetTypeInfo());
        }
    }
}