#if NETCOREAPP3_1
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Controllers;
using JCTools.GenericCrud.Helpers;

namespace JCTools.GenericCrud.Settings.DependencyInjection
{

    /// <summary>
    /// Is responsible for the final <see cref="Endpoint"/> selection decision
    /// considering the CRUD controllers
    /// </summary>
    public class CrudEndpointSelector : EndpointSelector
    {
        /// <summary>
        /// The <see cref="EndpointSelector"/> configured instance 
        /// that replaced with the current instance
        /// </summary>
        private readonly EndpointSelector _defaultSelector;

        /// <summary>
        /// Initializes the current selector
        /// </summary>
        /// <param name="defaultSelector">The <see cref="EndpointSelector"/> 
        /// configured instance that replaced with the current instance</param>
        public CrudEndpointSelector(EndpointSelector defaultSelector)
            => _defaultSelector = defaultSelector;

        /// <summary>
        /// Asynchronously selects an <see cref="Endpoint"/> from the <see cref=" CandidateSet"/>.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> associated 
        /// with the current request.</param>
        /// <param name="candidates">The possible candidates</param>
        /// <returns>The task to be executed</returns>
        public override Task SelectAsync(HttpContext httpContext, CandidateSet candidates)
        {
            // check duplicate candidates
            // TODO: both endpoint are equals but the controller not are equals
            CandidateState current = candidates[0];
            var distinctCandidates = new List<CandidateState>() { current };
            for (int i = 1; i < candidates.Count; i++)
            {
                var differences = ObjectComparer.DetailedCompare(current, candidates[i], depth: 10);
                if (differences.Any())
                    distinctCandidates.Add(candidates[i]);
            }

            if (distinctCandidates.Count() == 1)
                return SetCandidate(httpContext, distinctCandidates[0]);

            foreach (var candidate in distinctCandidates)
            {
                if (candidate.Endpoint is RouteEndpoint endpoint)
                {
                    var realControllerType = endpoint.Metadata
                        .GetMetadata<ControllerActionDescriptor>()
                        ?.ControllerTypeInfo;

                    if (TryFindGenericController(realControllerType, out Type controllerType)
                        && controllerType.GenericTypeArguments.Length == 3)
                    {
                        var args = controllerType.GenericTypeArguments.Skip(1);
                        var controllerCrudType = Configurator.Options
                            .Models[args.First(), args.Last()];

                        if (candidate.Values.TryGetValue(Configurator.ICrudTypeTokenName, out object value)
                            && value is ICrudType candidateCrudType)
                        {
                            var areEquals = controllerCrudType.ModelType.Equals(candidateCrudType.ModelType)
                                && controllerCrudType.KeyPropertyName.Equals(candidateCrudType.KeyPropertyName)
                                && controllerCrudType.KeyPropertyType.Equals(candidateCrudType.KeyPropertyType)
                                && controllerCrudType.ControllerType.Equals(candidateCrudType.ControllerType);

                            if (areEquals)
                                return SetCandidate(httpContext, candidate);
                        }
                    }
                }
            }

            return _defaultSelector.SelectAsync(httpContext, candidates);
        }

        /// <summary>
        /// Sets the selected candidate to the HTTP context 
        /// </summary>
        /// <param name="httpContext">The HTTP context to be affected</param>
        /// <param name="candidate">The selected candidate to set</param>
        /// <returns>A task completed</returns>
        private static Task SetCandidate(HttpContext httpContext, CandidateState candidate)
        {
            httpContext.SetEndpoint(candidate.Endpoint);
            httpContext.Request.RouteValues = candidate.Values;
            return Task.CompletedTask;
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
            if (controllerType == null)
                resultType = null;
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