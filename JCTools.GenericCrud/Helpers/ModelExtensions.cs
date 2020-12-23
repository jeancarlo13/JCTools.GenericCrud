using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JCTools.GenericCrud.DataAnnotations;
using JCTools.GenericCrud.Models;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Helpers
{
    /// <summary>
    /// Extensors used for get the model properties
    /// </summary>
    public static class ModelExtensions
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
        /// Allows get all properties to show into the Crud list
        /// </summary>
        /// <returns>Collection of the properties names to show</returns>
        public static IEnumerable<string> GetModelColumns<TModel, TKey>(this Base<TModel, TKey> list, IStringLocalizer localizer)
            where TModel : class, new()
            => GetPropertiesList<TModel, TKey>(list).Select(p => GetPropertyDisplay(p, localizer));

        private static string GetPropertyDisplay(Property p, IStringLocalizer localizer)
        {
            if (p.Display == null)
                return p.Info.Name;
            else
            {
                var name = p.Display.GetName();
                return string.IsNullOrWhiteSpace(name) ? p.Info.Name : localizer[name];
            }
        }
        public static IEnumerable<IEnumerable<Data>> GetModelValues(this ICrudList list)
        {
            var properties = InvokePropertiesListMethod(list);
            return list.GetData()
                .Select(d =>
                    properties.Select(p => new Data
                    {
                        Name = p.Info.Name,
                        Value = p.Info.GetValue(d),
                        Visible = p.List?.Visible ?? true,
                        UseCustomView = p.List?.UseCustomView ?? false
                    })
                );
        }
        public static IEnumerable<Data> GetModelValues(this IBaseDetails details)
        {
            var model = details.GetData();
            var properties = InvokePropertiesListMethod(details);
            return properties.Select(p => new Data
            {
                Name = p.Info.Name,
                Display = GetPropertyDisplay(p, details.Localizer),
                Value = model != null ? p.Info.GetValue(model) : null,
                Visible = p.List?.Visible ?? true,
                UseCustomView = p.List?.UseCustomView ?? false
            });
        }
        
        /// <summary>
        /// Located and invoke the correct <see cref="GetPropertiesList{TModel, TKey}(IBase, bool)"/> method
        /// </summary>
        /// <param name="config">The object to be used into the method invocation</param>
        /// <returns>The found collection of properties</returns>
        private static IEnumerable<Property> InvokePropertiesListMethod(IBase config)
        {
            MethodInfo method = typeof(ModelExtensions)
                .GetMethod(
                    nameof(GetPropertiesList),
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
                );
            MethodInfo generic = method.MakeGenericMethod(config.GetModelGenericType(), config.GetKeyGenericType());
            return generic.Invoke(null, new object[] { config, true }) as IEnumerable<Property>;
        }

        /// <summary>
        /// Allows get the properties that should appear into the crud list    
        /// </summary>
        /// <param name="includeNoVisibleColumns">True for include the not visible columns; False for return only the visible columns</param>
        /// <param name="config">The configuration of the list</param>
        /// <returns>Collection of properties</returns>
        private static IEnumerable<Property> GetPropertiesList<TModel, TKey>(IBase config, bool includeNoVisibleColumns = false)
            where TModel : class, new()
        {
            return config.GetModelGenericType().GetTypeInfo().GetProperties()
                .Select(p => new Property
                {
                    Info = p,
                    List = p.GetCustomAttribute<CrudAttribute>(),
                    Display = p.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                })
                .Where(p => includeNoVisibleColumns || (p.List?.Visible ?? true))
                .OrderBy(p => p.Display?.Order ?? 0);
        }

    }
}