using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Provides an mechanics for selecting an MVC action to invoke for the current request.
    /// </summary>
    public partial class CrudActionSelector : IActionSelector
    {
        /// <summary>
        /// Empty collection to be used when haven't matches
        /// </summary>
        private static readonly IReadOnlyList<ActionDescriptor> EmptyActions = Array.Empty<ActionDescriptor>();
        /// <summary>
        /// Used for get the access to the action descriptors
        /// </summary>
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        /// <summary>
        /// Allows access to the cache of the action constraints
        /// </summary>
        private readonly ActionConstraintCache _actionConstraintCache;

        /// <summary>
        /// The logger instance to be use for send the app messages 
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Store the current cache of the action selector
        /// </summary>
        private CrudActionSelectorCache _cache;

        /// <summary>
        /// Creates a new <see cref="ActionSelector"/>.
        /// </summary>
        /// <param name="actionDescriptorCollectionProvider">The <see cref="IActionDescriptorCollectionProvider"/> 
        /// to be used for get the access to the action descriptors </param>
        /// <param name="actionConstraintCache">The <see cref="ActionConstraintCache"/> that providers 
        /// a set of <see cref="IActionConstraint"/> instances.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to be use for generate the logger instance</param>
        public CrudActionSelector(
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            ActionConstraintCache actionConstraintCache,
            ILoggerFactory loggerFactory)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _logger = loggerFactory.CreateLogger<ActionSelector>();
            _actionConstraintCache = actionConstraintCache;
        }

        /// <summary>
        /// Allows access to the current cache of the action selector
        /// </summary>
        private CrudActionSelectorCache GetCurrentCache()
        {
            var actions = _actionDescriptorCollectionProvider.ActionDescriptors;
            var cache = Volatile.Read(ref _cache);

            if (cache != null && cache.Version == actions.Version)
                return cache;

            cache = new CrudActionSelectorCache(actions);
            Volatile.Write(ref _cache, cache);
            return cache;
        }
        /// <summary>
        /// Selects a set of <see cref="ActionDescriptor"/> candidates for the current request associated with context.
        /// </summary>
        /// <param name="context">The <see cref="RouteContext"/> associated with the current request.</param>
        /// <returns>A set of <see cref="ActionDescriptor"/> candidates or null.</returns>
        public IReadOnlyList<ActionDescriptor> SelectCandidates(RouteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var cache = GetCurrentCache();

            // The Cache works based on a string[] of the route values in a pre-calculated order. This code extracts
            // those values in the correct order.
            var values = cache.RouteKeys
                .Select(k =>
                {
                    if (context.RouteData.Values.TryGetValue(k, out object value))
                        return value as string ?? Convert.ToString(value);
                    return null;
                })
                .ToArray();

            if (cache.OrdinalEntries.TryGetValue(values, out var matchingRouteValues) ||
                cache.OrdinalIgnoreCaseEntries.TryGetValue(values, out matchingRouteValues))
            {
                Debug.Assert(matchingRouteValues != null);
                return matchingRouteValues;
            }

            _logger.LogDebug($"No actions matched the current request. Route values: {context.RouteData.Values}");
            return EmptyActions;
        }
        /// <summary>
        /// Selects the best <see cref="ActionDescriptor"/> candidate from candidates 
        /// for the current request associated with context.
        /// </summary>
        /// <param name="context">The <see cref="RouteContext"/> associated with the current request</param>
        /// <param name="candidates">The set of <see cref="ActionDescriptor"/> candidates to be evaluated</param>
        /// <returns>The best <see cref="ActionDescriptor"/> candidate for the current request or null</returns>
        /// <exception cref="AmbiguousActionException">Thrown when action selection results in an ambiguity</exception>
        /// <exception cref="ArgumentNullException">Thrown when the context or the candidates arguments are nulls</exception>
        public ActionDescriptor SelectBestCandidate(RouteContext context, IReadOnlyList<ActionDescriptor> candidates)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (candidates == null)
                throw new ArgumentNullException(nameof(candidates));

            var matches = SelectBestActions(EvaluateActionConstraints(context, candidates));
            if (matches == null || matches.Count == 0)
                return null;
            else if (matches.Count == 1)
                return matches[0];
            else
            {
                matches = MatchCrudActions(context, candidates);
                if (matches.Count == 1)
                    return matches[0];
                else
                {
                    var actionNames = string.Join(
                        Environment.NewLine,
                        matches.Select(a => a.DisplayName));

                    _logger.LogError($"Request matched multiple actions resulting in ambiguity. Matching actions: {actionNames}");

                    var message = "Request matched multiple actions resulting in ambiguity. Matching actions:"
                        + Environment.NewLine
                        + actionNames;

                    throw new AmbiguousActionException(message);
                }
            }
        }
        /// <summary>
        /// Find the match of the current request with the CRUD controllers
        /// </summary>
        /// <param name="context">The <see cref="RouteContext"/> associated with the current request</param>
        /// <param name="candidates">The set of <see cref="ActionDescriptor"/> candidates to be evaluated</param>
        /// <returns>The best <see cref="ActionDescriptor"/> candidate for the current request or all candidate if not match</returns>
        private IReadOnlyList<ActionDescriptor> MatchCrudActions(RouteContext context, IReadOnlyList<ActionDescriptor> candidates)
        {
            var requestedModelType = context.RouteData.DataTokens[Configurator.ModelTypeTokenName] as Type;

            var results = candidates
                .Where(c =>
                {
                    var candidate = c as ControllerActionDescriptor;
                    if (candidate == null)
                        return false;

                    var genericTypes = candidate.ControllerTypeInfo.GetGenericArguments();
                    return (requestedModelType != null && genericTypes.Contains(requestedModelType))
                        || candidate.ControllerName != Configurator.GenericControllerType.Name;
                })
                .ToList();

            return results.Any() ? results : candidates;
        }

        /// <summary>
        /// Returns the set of best matching actions.
        /// </summary>
        /// <param name="actions">The set of actions that satisfy all constraints.</param>
        /// <returns>A list of the best matching actions.</returns>
        protected virtual IReadOnlyList<ActionDescriptor> SelectBestActions(IReadOnlyList<ActionDescriptor> actions)
            => actions;

        /// <summary>
        /// Tries find the candidate with more constraints coincidences for the current request
        /// </summary>
        /// <param name="context">The <see cref="RouteContext"/> associated with the current request.</param>
        /// <param name="actions">The set of <see cref="ActionDescriptor"/> candidates to be evaluated</param>
        /// <returns>The candidates that match with the current request or null</returns>
        private IReadOnlyList<ActionDescriptor> EvaluateActionConstraints(
            RouteContext context,
            IReadOnlyList<ActionDescriptor> actions)
        {
            var candidates = actions.Select(action =>
            {
                var constraints = _actionConstraintCache.GetActionConstraints(context.HttpContext, action);
                return new ActionSelectorCandidate(action, constraints);
            }).ToList();

            var matches = EvaluateActionConstraintsCore(context, candidates, startingOrder: null);

            return matches?
                .Select(candidate => candidate.Action)
                .ToList();
        }
        /// <summary>
        /// Tries find the candidate with more constraints coincidences for the current request
        /// </summary>
        /// <param name="context">The <see cref="RouteContext"/> associated with the current request.</param>
        /// <param name="candidates">The candidates for the selection of the action to invoke for the current request</param>
        /// <param name="startingOrder">The candidate index to be used for start the search</param>
        /// <returns>The candidates that match with the current request</returns>
        private IReadOnlyList<ActionSelectorCandidate> EvaluateActionConstraintsCore(
            RouteContext context,
            IReadOnlyList<ActionSelectorCandidate> candidates,
            int? startingOrder)
        {
            // Find the next group of constraints to process. This will be the lowest value of
            // order that is higher than startingOrder.
            int? order = null;

            // Perf: Avoid allocations
            for (var i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates[i];
                if (candidate.Constraints != null)
                {
                    for (var j = 0; j < candidate.Constraints.Count; j++)
                    {
                        var constraint = candidate.Constraints[j];
                        if ((startingOrder == null || constraint.Order > startingOrder) &&
                            (order == null || constraint.Order < order))
                        {
                            order = constraint.Order;
                        }
                    }
                }
            }

            // If we don't find a next then there's nothing left to do.
            if (order == null)
                return candidates;

            // Since we have a constraint to process, bisect the set of actions into those with and without a
            // constraint for the current order.
            var actionsWithConstraint = new List<ActionSelectorCandidate>();
            var actionsWithoutConstraint = new List<ActionSelectorCandidate>();

            var constraintContext = new ActionConstraintContext();
            constraintContext.Candidates = candidates;
            constraintContext.RouteContext = context;

            // Perf: Avoid allocations
            for (var i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates[i];
                var isMatch = true;
                var foundMatchingConstraint = false;

                if (candidate.Constraints != null)
                {
                    constraintContext.CurrentCandidate = candidate;
                    for (var j = 0; j < candidate.Constraints.Count; j++)
                    {
                        var constraint = candidate.Constraints[j];
                        if (constraint.Order == order)
                        {
                            foundMatchingConstraint = true;

                            if (!constraint.Accept(constraintContext))
                            {
                                isMatch = false;
                                _logger.LogDebug($"Action '{candidate.Action.DisplayName}' with id '{candidate.Action.Id}' did not match the constraint '{constraint}'");
                                break;
                            }
                        }
                    }
                }

                if (isMatch && foundMatchingConstraint)
                    actionsWithConstraint.Add(candidate);
                else if (isMatch)
                    actionsWithoutConstraint.Add(candidate);
            }

            // If we have matches with constraints, those are better so try to keep processing those
            if (actionsWithConstraint.Count > 0)
            {
                var matches = EvaluateActionConstraintsCore(context, actionsWithConstraint, order);
                if (matches?.Count > 0)
                    return matches;
            }

            // If the set of matches with constraints can't work, then process the set without constraints.
            if (actionsWithoutConstraint.Count == 0)
                return null;
            else
                return EvaluateActionConstraintsCore(context, actionsWithoutConstraint, order);
        }
    }

}