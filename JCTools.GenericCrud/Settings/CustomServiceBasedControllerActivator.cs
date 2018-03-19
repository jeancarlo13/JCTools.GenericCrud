using System;
using JCTools.GenericCrud.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace JCTools.GenericCrud.Settings
{
    public class CustomServiceBasedControllerActivator : IControllerActivator
    {
        private readonly IServiceProvider _serviceProvider;
        public CustomServiceBasedControllerActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public object Create(ControllerContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (actionContext.RouteData.Values["controller"]?.ToString().Equals("GenericController`3") ?? false)
            {
                var modelType = actionContext.RouteData.DataTokens[Configurator.ModelTypeTokenName] as Type;
                var keyName = actionContext.RouteData.DataTokens[Configurator.KeyTokenName]?.ToString() ?? "Id";
                var controller = _serviceProvider.CreateGenericController(modelType, keyName);
                return controller;
            }

            var controllerType = actionContext.ActionDescriptor.ControllerTypeInfo.AsType();
            return actionContext.HttpContext.RequestServices.GetRequiredService(controllerType);
        }

        public void Release(ControllerContext context, object controller)
        { }
    }
}