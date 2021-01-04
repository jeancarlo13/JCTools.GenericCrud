using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.Linq;

namespace JCTools.GenericCrud.Settings.DependencyInjection
{
    internal class CrudModelBinder : IModelBinder
    {
        /// <summary>
        /// The collection of the model binder to be used to bind the models
        /// </summary>
        private readonly IEnumerable<CrudModelBinderMetadata> _binders;
        
        /// <summary>
        /// Initialize the instance with specified <see cref="CrudModelBinderMetadata"/> collection
        /// </summary>
        /// <param name="binders">The <see cref="CrudModelBinderMetadata"/> collection to be used 
        /// to bind the CRUD models</param>
        public CrudModelBinder(IEnumerable<CrudModelBinderMetadata> binders)
            => _binders = binders ?? throw new ArgumentNullException(nameof(binders));
        
        /// <summary>
        /// Attempts to bind a model.
        /// </summary>
        /// <param name="bindingContext">The <see cref="ModelBindingContext"/> with the CRUD model data to be used.</param>
        /// <returns>The task to be execute</returns>
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext is null)
                throw new ArgumentNullException(nameof(bindingContext));

            if (bindingContext.FieldName.Equals(Constants.EntitySettingsRouteKey))
            {
                var modelName = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
                if (modelName.Length == 1)
                {
                    var crudType = Configurator.Options.Models[modelName.FirstValue];
                    bindingContext.Result = ModelBindingResult.Success(crudType);
                }
            }
            else if (bindingContext.FieldName.Equals(Constants.EntityModelRouteKey))
            {
                var routeValues = (bindingContext as DefaultModelBindingContext)?.ActionContext.RouteData.Values;
                var crudType = Configurator.Options.Models[routeValues];
                if (crudType != null)
                {
                    var binder = _binders.FirstOrDefault(b => b.Type.Equals(crudType.ModelType));
                    if (binder != null)
                    {
                        var newBindingContext = DefaultModelBindingContext.CreateBindingContext(
                            bindingContext.ActionContext,
                            bindingContext.ValueProvider,
                            binder.Metadata,
                            bindingInfo: null,
                            bindingContext.ModelName
                        );

                        await binder.Binder.BindModelAsync(newBindingContext);
                        bindingContext.Result = newBindingContext.Result;

                        if (newBindingContext.Result.IsModelSet)
                        {
                            // Setting the ValidationState ensures properties on derived types are correctly 
                            bindingContext.ValidationState[newBindingContext.Result] = new ValidationStateEntry
                            {
                                Metadata = binder.Metadata,
                            };
                        }
                    }
                }
            }
        }
    }
}