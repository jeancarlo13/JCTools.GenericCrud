using Microsoft.Extensions.Localization;

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
    }
}