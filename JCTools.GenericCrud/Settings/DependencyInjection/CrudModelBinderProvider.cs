#if NETCOREAPP3_1
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace JCTools.GenericCrud.Settings.DependencyInjection
{
    /// <summary>
    /// Create the required <see cref="IModelBinder"/> instances for the creation of the CRUD models
    /// </summary>
    public class CrudModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Creates a <see cref="IModelBinder"/> based on <see cref="ModelBinderProviderContext"/> 
        /// and the configred CRUD types 
        /// </summary>
        /// <param name="context">The <see cref="ModelBinderProviderContext"/> to be used.</param>
        /// <returns>The created <see cref="IModelBinder"/></returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var argName = context.Metadata.Name;
            var isSettings = !string.IsNullOrWhiteSpace(argName)
                && argName.Equals(Settings.Constants.EntitySettingsRouteKey);
            var isModel = !string.IsNullOrWhiteSpace(argName)
                && argName.Equals(Settings.Constants.EntityModelRouteKey);

            if (isSettings || isModel)
            {
                var binders = new List<CrudModelBinderMetadata>();

                binders.Add(new CrudModelBinderMetadata(typeof(ICrudType), context));

                foreach (var type in Configurator.Options.Models.ToList())
                    binders.Add(new CrudModelBinderMetadata(type.ModelType, context));

                return new CrudModelBinder(binders);
            }

            return null;
        }
    }
}
#endif