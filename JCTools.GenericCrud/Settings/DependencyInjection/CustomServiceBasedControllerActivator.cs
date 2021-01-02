using System;
using System.Linq;
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

            var controllerType = actionContext?.ActionDescriptor?.ControllerTypeInfo;
            ICrudType crud;
            if (controllerType?.Name.Equals(Configurator.GenericControllerType.Name) ?? false
                && controllerType.GenericTypeArguments.Length == 3)
            {
                var args = controllerType.GenericTypeArguments.Skip(1);
                crud = Configurator.Options.Models[args.First(), args.Last()];
            }
            else if (actionContext.RouteData.Values["controller"]?.ToString().Equals(Configurator.GenericControllerType.Name) ?? false)
                crud = Configurator.Options.Models[actionContext.RouteData.DataTokens];
            else
            {
                controllerType = actionContext.ActionDescriptor.ControllerTypeInfo;
                return actionContext.HttpContext.RequestServices.GetRequiredService(controllerType);
            }

            if (crud == null)
                throw new InvalidOperationException($"The \"{actionContext.RouteData.Values["controller"]}\" is not appropriated registered.");
                
            return crud.GetControllerInstance(actionContext.HttpContext.RequestServices);
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