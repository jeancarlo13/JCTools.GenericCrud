using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JCTools.GenericCrud.DataAnnotations;
using JCTools.GenericCrud.Models;
using JCTools.GenericCrud.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Helpers
    {
        /// <summary>
        /// Extensors used for get the model properties
        /// </summary>
        public static class ModelExtensions
        {
            internal static string GetLocalizedString(this IStringLocalizer localizer, string key, params string[] parameters) => GetLocalizedString(localizer, key, null, parameters);

            internal static string GetLocalizedString(this IStringLocalizer localizer, string key, string @default, params string[] parameters){
            var localized = localizer[key].Value;

            if (!string.IsNullOrWhiteSpace(@default) && localized == key)
                localized = @default;

            return string.Format(localized, parameters);
        }
        /// <summary>
        /// Allows get all properties to show into the Crud list
        /// </summary>
        /// <returns>Collection of the properties names to show</returns>
        public static IEnumerable<string> GetModelColumns<TModel, TKey>(this Base<TModel, TKey> list, IStringLocalizer localizer)
            where TModel : class, new()
            => GetListProperties<TModel, TKey>(list).Select(p => GetPropertyDisplay(p, localizer));
            
        private static string GetPropertyDisplay(Property p, IStringLocalizer localizer)
        {
            if (p.Display == null)
                return p.Info.Name;
            else {
                var name = p.Display.GetName();
                return string.IsNullOrWhiteSpace(name)? p.Info.Name : localizer[name];
            }
        }
        public static IEnumerable<IEnumerable<Data>> GetModelValues(this ICrudList list)
        {
            var properties = InvokeListProperties(list);
            return list.GetData()
                .Select(d => 
                    properties.Select(p => new  Data
                    {
                        Name = p.Info.Name,
                        Value = p.Info.GetValue(d),
                        Visible = p.List?.Visible ?? true
                    })
                );
        }
        public static IEnumerable<Data> GetModelValues(this IBaseDetails details)
        {
            var model =  details.GetData();
            var properties = InvokeListProperties(details);
            return properties.Select(p => new  Data
            {
                Name = p.Info.Name,
                Display = GetPropertyDisplay(p, details.Localizer),
                Value = p.Info.GetValue(model),
                Visible = p.List?.Visible ?? true
            });
        }
        private static IEnumerable<Property> InvokeListProperties(IBase config)
        {
            MethodInfo method = typeof(ModelExtensions)
                .GetMethod(
                    nameof(GetListProperties), 
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
                );
            MethodInfo generic = method.MakeGenericMethod(config.GetModelGenericType(), config.GetKeyGenericType());
           return generic.Invoke(null, new object[]{config, true}) as IEnumerable<Property>;            
        }
        /// <summary>
        /// Allows get the properties that shuld appear into the crud list    
        /// </summary>
        /// <param name="includeNoVisibles">True for include the not visible collumns; False for return only the visible columns</param>
        /// <param name="config">The configuration of the list</param>
        /// <returns>Collection of properties</returns>
        private static IEnumerable<Property> GetListProperties<TModel, TKey>(IBase config, bool includeNoVisibles = false)
            where TModel : class, new()
        {
            return config.GetModelGenericType().GetTypeInfo().GetProperties()
                .Select(p => new Property
                {
                    Info = p,
                    List = p.GetCustomAttribute<CrudAttribute>(),
                    Display = p.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                })
                .Where(p => includeNoVisibles || (p.List?.Visible ?? true))
                .OrderBy(p => p.Display?.Order ?? 0);
        }

    }
}