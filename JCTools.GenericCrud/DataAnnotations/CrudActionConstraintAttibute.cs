#if NETCOREAPP3_1
using System;
using System.Linq;
using System.Reflection;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;

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
            if (crudType == null)
                return false;

            var accept = actionDescriptor.ControllerTypeInfo.Equals(crudType.ControllerType.GetTypeInfo());
            
            return accept;
        }
    }
}
#endif