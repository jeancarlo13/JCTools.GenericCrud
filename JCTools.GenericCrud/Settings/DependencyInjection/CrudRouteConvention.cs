using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace JCTools.GenericCrud.Settings.DependencyInjection
{
    internal class CrudRouteConvention : IControllerModelConvention
    {
        private IList<Type> _controllers;

        public CrudRouteConvention(IReadOnlyList<ICrudType> controllers)
        {
            _controllers = controllers.ToList()
                .Select(c => c.ControllerType)
                .ToList();
        }

        public CrudRouteConvention AddController(Type controller)
        {
            this._controllers.Add(controller);
            return this;
        }

        public void Apply(ControllerModel controller)
        {
            var result = _controllers
                .Where(x => x.Name.Equals(string.Format("{0}Controller", controller.ControllerName)))
                .SingleOrDefault();

            if (result == null)
            {
                if (controller.Selectors.Any(selector => selector.AttributeRouteModel != null))
                {
                    foreach (var controllerSelector in controller.Selectors.Where(x => x.AttributeRouteModel != null))
                    {
                        var originalTemp = controllerSelector.AttributeRouteModel.Template;

                        var newTemplate = new StringBuilder();

                        newTemplate.Append(PascalToKebabCase(controller.ControllerName));

                        controllerSelector.AttributeRouteModel = new AttributeRouteModel
                        {
                            Template = originalTemp.Replace("[controller]", newTemplate.ToString())
                        };

                        foreach (var controllerAction in controller.Actions)
                        {
                            var withRouteAttributes = controllerAction.Selectors
                                .Where(x => x.AttributeRouteModel != null);
                            foreach (var actionselector in withRouteAttributes)
                            {
                                var origTemp = actionselector.AttributeRouteModel.Template;

                                var template = new StringBuilder();

                                template.Append(PascalToKebabCase(controllerAction.ActionName));

                                actionselector.AttributeRouteModel = new AttributeRouteModel
                                {
                                    Template = origTemp.Replace("[action]", template.ToString())
                                };
                            }
                        }
                    }
                }
            }
        }

        public static string PascalToKebabCase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(
                    value.ToString(),
                   "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                   "-$1",
                   RegexOptions.Compiled)
                   .Trim()
                   .ToLower();
        }
    }
}