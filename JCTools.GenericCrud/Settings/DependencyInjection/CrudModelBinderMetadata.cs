#if NETCOREAPP3_1
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JCTools.GenericCrud.Settings.DependencyInjection
{
    /// <summary>
    /// Defines the required data for bind a model
    /// </summary>
    internal class CrudModelBinderMetadata
    {
        /// <summary>
        /// The type of the related model to bind
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// The <see cref="ModelMetadata"/> of the related model to bind
        /// </summary>
        public ModelMetadata Metadata { get; set; }
        /// <summary>
        /// The <see cref="IModelBinder"/> of the related model to bind
        /// </summary>
        public IModelBinder Binder { get; set; }

        /// <summary>
        /// Generate a new instance with the found data
        /// </summary>
        /// <param name="type">The type to relate to the current instance.</param>
        /// <param name="context">The context with the metadata of the instance.</param>
        public CrudModelBinderMetadata(Type type, ModelBinderProviderContext context)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));

            if (context is null)
                throw new ArgumentNullException(nameof(context));
            Metadata = context.MetadataProvider.GetMetadataForType(type);
            Binder = context.CreateBinder(Metadata);
        }
    }
}
#endif