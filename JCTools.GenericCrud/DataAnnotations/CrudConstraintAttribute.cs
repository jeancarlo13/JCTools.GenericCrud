#if NETCOREAPP3_1
using System;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using JCTools.GenericCrud.Controllers;

namespace JCTools.GenericCrud.DataAnnotations
{
    /// <summary>
    /// Allows discriminating between multiple CRUD endpoints 
    /// </summary>
    /// <remarks>See remarks on <see cref="IActionConstraint"/>.</remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class CrudConstraintAttribute : Attribute, IActionConstraint
    {
        /// <summary>
        /// The constraint order.
        /// </summary>
        /// <Remarks>Constraints are grouped into stages by the value of <see cref="IActionConstraint.Order"/>.
        /// See remarks on <see cref="IActionConstraint"/>.</Remarks>
        public int Order { get; set; }

        /// <summary>
        /// The type of the CRUD to the related controller
        /// </summary>
        internal ICrudType CrudType { get; private set; }

        /// <summary>
        /// Initializes the attribute
        /// </summary>
        internal CrudConstraintAttribute() { }

        /// <summary>
        /// Initializes the attribute for a custom controller
        /// </summary>
        public CrudConstraintAttribute(Type modelType, string keyProperty = "Id")
        {
            if (string.IsNullOrWhiteSpace(keyProperty))
                throw new ArgumentException(
                    $"'{nameof(keyProperty)}' cannot be null or whitespace.", nameof(keyProperty));

            CrudType = Configurator.Options.Models[modelType, keyProperty];

            if (CrudType == null)
                throw new ArgumentException(
                    $"Not exists a configured Crud for the model \"{modelType}\" and the key property \"{keyProperty}\"");

        }

        /// <summary>
        /// Determines whether an action is a valid candidate for selection 
        /// considering the configure model type por the controller
        /// </summary>
        /// <param name="context">The <see cref="ActionConstraintContext"/> with the request data.</param>
        /// <returns>True if the action is valid for selection, otherwise false.</returns>
        public bool Accept(ActionConstraintContext context)
        {
            var accept = false;
            var actionDescriptor = context.CurrentCandidate.Action as ControllerActionDescriptor;
            if (actionDescriptor != null)
            {
                var modelType = context.RouteContext.RouteData.Values[Constants.EntitySettingsRouteKey]?.ToString()
                    ?? context.RouteContext.RouteData.Values[Configurator.ModelTypeTokenName]?.ToString();

                var _crud = Configurator.Options.Models[modelType];

                if (CrudType == null)
                    accept = _crud?.UseGenericController ?? false;
                else
                    accept = CrudType.ModelType.Equals(_crud.ModelType)
                        && actionDescriptor.ControllerTypeInfo.Equals(CrudType.ControllerType);
            }

            return accept;
        }

        /// <summary>
        /// Try find the <see cref="Controllers.GenericController"/> definition used into the specified type
        /// </summary>
        /// <param name="controllerType">The controller type to review</param>
        /// <param name="resultType">The found <see cref="Controllers.GenericController"/> definition</param>
        /// <returns>True if the <see cref="Controllers.GenericController"/> definition is found, else false</returns>
        private bool TryFindGenericController(Type controllerType, out Type resultType)
        {
            if (controllerType.Equals(typeof(GenericController)))
                resultType = controllerType;
            else if (controllerType.BaseType == null)
                resultType = null;
            else
                return TryFindGenericController(controllerType.BaseType, out resultType);

            return resultType != null;
        }

    }
}
#endif