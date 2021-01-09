using System;
using System.Resources;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace JCTools.GenericCrud.Services
{
    /// <summary>
    /// Represents a service that provides localized strings for the CRUDs
    /// </summary>
    internal class CrudLocalizer : ICrudLocalizer
    {
        /// <summary>
        /// An <see cref="IStringLocalizer"/> that uses the <see cref="ResourceManager"/> and
        /// <see cref="ResourceReader"/> to provide localized strings.
        /// </summary>
        private readonly ResourceManagerStringLocalizer _resourcesManager;

        /// <summary>
        /// Initialize the current instance
        /// </summary>
        /// <param name="resourcesManager">A strongly-typed resource class, for looking up localized strings</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to be used for create 
        /// the required <see cref="ILogger"/>.</param>
        public CrudLocalizer(
            ResourceManager resourcesManager,
            ILoggerFactory loggerFactory
        )
        {
            if (resourcesManager is null)
                throw new ArgumentNullException(nameof(resourcesManager));

            if (loggerFactory is null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _resourcesManager = new ResourceManagerStringLocalizer(
                resourcesManager,
                resourcesManager.ResourceSetType.Assembly,
                resourcesManager.BaseName ?? string.Empty,
                new ResourceNamesCache(),
                loggerFactory.CreateLogger<CrudLocalizer>()
            );
        }

        /// <summary>
        /// Gets the string resource with the given name.
        /// </summary>
        /// <param name="name"> The name of the string resource.</param>
        /// <returns>The string resource as a <see cref="LocalizedString"/>.</returns>
        public LocalizedString this[string name] => _resourcesManager[name];

        /// <summary>
        /// Gets the string resource with the given name and formatted with the supplied arguments.
        /// </summary>
        /// <param name="name"> The name of the string resource.</param>
        /// <param name="arguments">The values to format the string with.</param>
        /// <returns> The formatted string resource as a <see cref="LocalizedString"/>.</returns>  
        public LocalizedString this[string name, params object[] arguments]
            => _resourcesManager[name, arguments];

        /// <summary>
        /// Gets all string resources.
        /// </summary>
        /// <param name="includeParentCultures">True for indicating whether to include strings 
        /// from parent cultures; else, false.</param>
        /// <returns>The found strings.</returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            => _resourcesManager.GetAllStrings(includeParentCultures);

#if NETCOREAPP2_1 || NETCOREAPP3_1
        /// <summary>
        /// Creates a new ResourceManagerStringLocalizer for a specific <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="culture">The <see cref="CultureInfo"/> to use.</param>
        /// <returns>A culture-specific <see cref="ResourceManagerStringLocalizer"/>.</returns>
        [Obsolete("This method is obsolete. Use `CurrentCulture` and `CurrentUICulture` instead.")]
        public IStringLocalizer WithCulture(CultureInfo culture)
            => _resourcesManager.WithCulture(culture);
#endif
    }
}