#if NETCOREAPP3_1
using System;
using System.Linq;
using System.Reflection;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Http.Extensions;

namespace JCTools.GenericCrud.DataAnnotations
{
    /// <summary>
    /// Allows discriminating between multiple CRUD endpoints 
    /// </summary>
    /// <remarks>See remarks on <see cref="IActionConstraint"/>.</remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CrudActionConstraintAttribute : Attribute, IActionConstraint
    {
        /// <summary>
        /// The constraint order.
        /// </summary>
        /// <Remarks>Constraints are grouped into stages by the value of <see cref="IActionConstraint.Order"/>.
        /// See remarks on <see cref="IActionConstraint"/>.</Remarks>
        public int Order { get; set; }

        /// <summary>
        /// Determines whether an action is a valid candidate for selection 
        /// considering the configure model type por the controller
        /// </summary>
        /// <param name="context">The <see cref="ActionConstraintContext"/> with the request data.</param>
        /// <returns>True if the action is valid for selection, otherwise false.</returns>
        public bool Accept(ActionConstraintContext context)
        {    
            var actionDescriptor = context.CurrentCandidate.Action as ControllerActionDescriptor;
            if (actionDescriptor == null)
                return false;

            var crudType = context.RouteContext.RouteData.Values[Configurator.ICrudTypeTokenName] as ICrudType;
            var modelType = context.RouteContext.RouteData.Values[Configurator.ModelTypeTokenName]?.ToString();
            var isFoundController = TryFindGenericController(actionDescriptor.ControllerTypeInfo, out Type controllerType);

            if (crudType == null && isFoundController)
            {
                var args = controllerType.GenericTypeArguments.Skip(1);
                crudType = Configurator.Options.Models[args.First(), args.Last()];
            }

            var accept = false;
            if (crudType != null)
                accept = actionDescriptor.ControllerTypeInfo.Equals(crudType.ControllerType.GetTypeInfo());
            else if (!string.IsNullOrWhiteSpace(modelType) && isFoundController)
            {
                accept = actionDescriptor.ControllerTypeInfo.GenericTypeArguments
                   .Any(gt => gt.Name.ToLowerInvariant().Equals(modelType.ToLowerInvariant()));
            }

            return accept;
        }

        /// <summary>
        /// Try find the <see cref="Controllers.GenericController{TContext, TModel, TKey}"/> definition
        /// used into the specified type
        /// </summary>
        /// <param name="controllerType">The controller type to review</param>
        /// <param name="resultType">The found <see cref="Controllers.GenericController{TContext, TModel, TKey}"/> definition</param>
        /// <returns>True if the <see cref="Controllers.GenericController{TContext, TModel, TKey}"/> definition is found, else false</returns>
        private bool TryFindGenericController(Type controllerType, out Type resultType)
        {
            if (controllerType.Name.Equals(Configurator.GenericControllerType.Name))
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