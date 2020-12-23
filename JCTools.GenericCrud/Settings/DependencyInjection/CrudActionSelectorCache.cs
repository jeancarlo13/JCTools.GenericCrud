using System;
using System.Collections.Generic;
using System.Linq;
using JCTools.GenericCrud.Helpers;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace JCTools.GenericCrud.Settings.DependencyInjection
{
    /// <summary>
    /// The action selector cache stores a mapping of route-values -> action descriptors for each known set of
    /// of route-values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The action selector cache stores a mapping of route-values -> action descriptors for each known set of
    /// of route-values. We actually build two of these mappings, one for case-sensitive (fast path) and one for
    /// case-insensitive (slow path).
    /// </para>
    /// <para>
    /// This is necessary because MVC routing/action-selection is always case-insensitive. So we're going to build
    /// a case-sensitive dictionary that will behave like the a case-insensitive dictionary when you hit one of the
    /// canonical entries. When you don't hit a case-sensitive match it will try the case-insensitive dictionary
    /// so you still get correct behaviors.
    /// </para>
    /// <para>
    /// The difference here is because while MVC is case-insensitive, doing a case-sensitive comparison is much 
    /// faster. We also expect that most of the URLs we process are canonically-cased because they were generated
    /// by Url.Action or another routing api.
    /// </para>
    /// <para>
    /// This means that for a set of actions like:
    /// </para>
    /// <para>
    ///      { controller = "Home", action = "Index" } -> HomeController::Index1()
    /// </para>
    /// <para>
    ///      { controller = "Home", action = "index" } -> HomeController::Index2()
    ///</para>
    /// <para>
    /// Both of these actions match "Index" case-insensitively, but there exist two known canonical casings,
    /// so we will create an entry for "Index" and an entry for "index". Both of these entries match **both**
    /// actions.
    /// </para>
    /// </remarks>
    internal class CrudActionSelectorCache
    {
        /// <summary>
        /// The version of the cache stored to invalidate it if actions change.
        /// </summary>
        public int Version { get; }
        /// <summary>
        /// An ordered set of keys for the route values. 
        /// </summary>
        /// <remarks>
        /// It'll use these later to extract the set of route values from an incoming request to compare against our maps of known route values.
        /// </remarks>            
        public string[] RouteKeys { get; }
        /// <summary>
        /// Route value map for the comparison using a case-sensitive ordinal string comparison.
        /// </summary>
        public Dictionary<string[], List<ActionDescriptor>> OrdinalEntries { get; }
        /// <summary>
        /// Route value map for the comparison using a case-insensitive ordinal string comparison.
        /// </summary>
        public Dictionary<string[], List<ActionDescriptor>> OrdinalIgnoreCaseEntries { get; }

        /// <summary>
        /// Initializes the cache instance with the received action descriptor collection
        /// </summary>
        /// <param name="actions">The action descriptor collection to be used for generate the new cache instance</param>
        public CrudActionSelectorCache(ActionDescriptorCollection actions)
        {
            Version = actions.Version;
            OrdinalEntries = new Dictionary<string[], List<ActionDescriptor>>(StringArrayComparer.Ordinal);
            OrdinalIgnoreCaseEntries = new Dictionary<string[], List<ActionDescriptor>>(StringArrayComparer.OrdinalIgnoreCase);

            // We need to first identify of the keys that action selection will look at (in route data). 
            // We want to only consider conventionally routed actions here.
            RouteKeys = new HashSet<string>(
                actions.Items
                    .Where(a => a.AttributeRouteInfo == null)
                    .SelectMany(a => a.RouteValues.Keys),
                StringComparer.OrdinalIgnoreCase
            ).ToArray();

            for (var i = 0; i < actions.Items.Count; i++)
            {
                var action = actions.Items[i];
                if (action.AttributeRouteInfo != null)
                {
                    // This only handles conventional routing. Ignore attribute routed actions.
                    continue;
                }

                // This is a conventionally routed action - so we need to extract the route values associated
                // with this action (in order) so we can store them in our dictionaries.
                var routeValues = new string[RouteKeys.Length];

                for (var j = 0; j < RouteKeys.Length; j++)
                    action.RouteValues.TryGetValue(RouteKeys[j], out routeValues[j]);

                if (!OrdinalIgnoreCaseEntries.TryGetValue(routeValues, out var entries))
                {
                    entries = new List<ActionDescriptor>();
                    OrdinalIgnoreCaseEntries.Add(routeValues, entries);
                }

                entries.Add(action);

                // We also want to add the same (as in reference equality) list of actions to the ordinal entries.
                // We'll keep updating `entries` to include all of the actions in the same equivalence class -
                // meaning, all conventionally routed actions for which the route values are equalignoring case.
                //
                // `entries` will appear in `OrdinalIgnoreCaseEntries` exactly once and in `OrdinalEntries` once
                // for each variation of casing that we've seen.
                if (!OrdinalEntries.ContainsKey(routeValues))
                    OrdinalEntries.Add(routeValues, entries);
            }
        }
    }

}