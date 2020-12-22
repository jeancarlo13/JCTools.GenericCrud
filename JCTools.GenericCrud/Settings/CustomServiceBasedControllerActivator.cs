using System;
using JCTools.GenericCrud.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Provides methods for generate the <see cref="Controllers.GenericController{TContext, TModel, TKey}" /> instances
    /// </summary>
    public class CustomServiceBasedControllerActivator : IControllerActivator
    {
        /// <summary>
        /// Creates a generic controller instance
        /// </summary>
        /// <param name="actionContext">The <see cref="ControllerContext"/> for the executing action.</param>
        /// <returns>The created instance</returns>
        public object Create(ControllerContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException(nameof(actionContext));

            if (actionContext.RouteData.Values["controller"]?.ToString().Equals(Configurator.GenericControllerType.Name) ?? false)
            {
                var modelType = actionContext.RouteData.DataTokens[Configurator.ModelTypeTokenName] as Type;
                var keyName = actionContext.RouteData.DataTokens[Configurator.KeyTokenName]?.ToString() ?? "Id";
                return actionContext.HttpContext.RequestServices.CreateGenericController(modelType, keyName);
            }

            var controllerType = actionContext.ActionDescriptor.ControllerTypeInfo.AsType();
            return actionContext.HttpContext.RequestServices.GetRequiredService(controllerType);
        }
        /// <summary>
        ///  Releases a controller.
        /// </summary>
        /// <remarks>Required for the <see cref="IControllerActivator"/> interface</remarks>
        /// <param name="context">The <see cref="ControllerContext"/> for the executing action.</param>
        /// <param name="controller">The controller to release.</param>
        public void Release(ControllerContext context, object controller) { }
    }
}