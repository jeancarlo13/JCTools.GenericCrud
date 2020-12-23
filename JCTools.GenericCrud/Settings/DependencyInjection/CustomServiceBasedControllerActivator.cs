using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace JCTools.GenericCrud.Settings.DependencyInjection
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
                var crud = Configurator.Options.Models[actionContext.RouteData.DataTokens];
                if (crud == null)
                    throw new InvalidOperationException($"The \"{actionContext.RouteData.Values["controller"]}\" is not appropriated registered.");

                return crud.GetControllerInstance(actionContext.HttpContext.RequestServices);
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