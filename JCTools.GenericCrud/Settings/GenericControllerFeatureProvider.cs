using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Allows configured the required controllers for generate the configured CRUDs 
    /// </summary>
    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        /// <summary>
        /// Register the GenericController into the application controller features
        /// </summary>
        /// <param name="parts">The list of Microsoft.AspNetCore.Mvc.ApplicationParts.ApplicationParts of the application.</param>
        /// <param name="feature">The feature instance to populate.</param>
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var cruds = Configurator.Options.Models.GetEnumerator();
            while (cruds.MoveNext())
            {
                var item = cruds.Current;
                var genericControllerType = Helpers.SettingsHelper.CreateGenericControllerType(item.Type, item.KeyPropertyName);
                feature.Controllers.Add(genericControllerType.GetTypeInfo());
            }
        }
    }
}