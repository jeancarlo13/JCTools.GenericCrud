using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace JCTools.GenericCrud.Helpers
{
    /// <summary>
    /// Extensors used for get the model properties
    /// </summary>
    public static class StringLocalizerExtensors
    {
        /// <summary>
        /// Allows get a localized string from the I18N/L12N resources
        /// </summary>
        /// <param name="localizer">The string localizer instance to be used by get the desired string</param>
        /// <param name="key">The name/key of the desired string</param>
        /// <param name="parameters">An object array that contains zero or more objects to format using the found localized string</param>
        /// <returns>The found localized string</returns>
        internal static string GetLocalizedString(this IStringLocalizer localizer, string key, params string[] parameters)
            => GetLocalizedString(localizer, key, null, parameters);
        /// <summary>
        /// Allows get a localized string from the I18N/L12N resources
        /// </summary>
        /// <param name="localizer">The string localizer instance to be used by get the desired string</param>
        /// <param name="key">The name/key of the desired string</param>
        /// <param name="default">The default value that will used if the desired string is not found</param>
        /// <param name="parameters">An object array that contains zero or more objects to format using the found localized string</param>
        /// <returns>The found localized string</returns>
        internal static string GetLocalizedString(this IStringLocalizer localizer, string key, string @default, params string[] parameters)
        {
            var localized = localizer[key].Value;

            if (string.IsNullOrWhiteSpace(localized) || localized == key)
                localized = @default;

            return string.Format(localized, parameters);
        }

        /// <summary>
        /// Returns a value that is used for display the type in the UI.
        /// </summary>
        /// <param name="localizer">The string localizer instance to be used by get the desired string</param>
        /// <typeparam name="TModel">The type to be use by search the localized string</typeparam>
        /// <returns>The localized string for the <see cref="DisplayAttribute.Name"/> property, 
        /// if the <see cref="DisplayAttribute.ResourceType"/> property has been specified and 
        /// the <see cref="DisplayAttribute.Name"/> property represents a resource key; otherwise, 
        /// the non-localized value of the <see cref="DisplayAttribute.Name"/> property.</returns>
        internal static string GetLocalizedString<TModel>(this IStringLocalizer localizer)
            where TModel : class
        {
            var type = typeof(TModel);
            var display = type.GetCustomAttribute<DisplayAttribute>(true);
            return display?.GetName() ?? type.Name;
        }

        /// <summary>
        /// Returns a value that is used for display the type in the UI.
        /// </summary>
        /// <param name="localizer">The string localizer instance to be used by get the desired string</param>
        /// <param name="propertyName">The name of the property that interests us</param>
        /// <typeparam name="TModel">The owner type of the property that interests us</typeparam>
        /// <returns>The localized string for the <see cref="DisplayAttribute.Name"/> property, 
        /// if the <see cref="DisplayAttribute.ResourceType"/> property has been specified and 
        /// the <see cref="DisplayAttribute.Name"/> property represents a resource key; otherwise, 
        /// the non-localized value of the <see cref="DisplayAttribute.Name"/> property.</returns>
        internal static string GetLocalizedString<TModel>(
            this IStringLocalizer localizer,
            string propertyName
        )
            where TModel : class
        {
            var property = typeof(TModel).GetProperty(propertyName);
            if (property == null)
                return propertyName;

            var display = property.GetCustomAttribute<DisplayAttribute>(true);
            return display?.GetName() ?? property.Name;
        }
    }
}