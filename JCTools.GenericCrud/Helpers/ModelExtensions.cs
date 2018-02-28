using System;
using JCTools.GenericCrud.Attibutes;
using JCTools.GenericCrud.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JCTools.GenericCrud.Settings;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Helpers
{
    /// <summary>
    /// Extensors used for get the model properties
    /// </summary>
    public static class ModelExtensions
    {
        private static string GetLocalizedString(this IStringLocalizer localizer, string key, params string[] parameters)
            => GetLocalizedString(localizer, key, null, parameters);
            
        private static string GetLocalizedString(this IStringLocalizer localizer, string key, string @default, params string[] parameters){
            var localized = localizer[key].Value;

            if (!string.IsNullOrWhiteSpace(@default) && localized == key)
                localized = @default;

            return string.Format(localized, parameters);
        }
        private static CrudAction ConfigureNewAction<TModel>(this ControllerOptions<TModel> options,string modelName, IStringLocalizer localizer)
            where TModel : class, new()
        {
            return new CrudAction()
            {
                Visible = options.AllowCreationAction,
                Caption = localizer.GetLocalizedString("GenericCrud.List.Create.Caption", "Create new {0}", modelName.ToLower()),
                Text = localizer.GetLocalizedString("GenericCrud.List.Create.Text", "Create"),
                IconClass = options?.Actions?.New?.IconClass ?? ActionOptions.DefaultNew.IconClass,
                ButtonClass = options?.Actions?.New?.ButtonClass ?? ActionOptions.DefaultNew.ButtonClass,
            };
        }
        private static CrudAction ConfigureDetailsAction<TModel>(this ControllerOptions<TModel> options,string modelName, IStringLocalizer localizer)
            where TModel : class, new()
        {
            return new CrudAction()
            {
                Visible = options.AllowShowDetailsAction,
                Caption = localizer.GetLocalizedString("GenericCrud.List.Details.Caption", "Details of the {0}", modelName.ToLower()),
                Text = localizer.GetLocalizedString("GenericCrud.List.Details.Text", "Details"),
                IconClass = options?.Actions?.Details?.IconClass ?? ActionOptions.DefaultDetails.IconClass,
                ButtonClass = options?.Actions?.Details?.ButtonClass ?? ActionOptions.DefaultDetails.ButtonClass,
            };
        }
        private static CrudAction ConfigureEditAction<TModel>(this ControllerOptions<TModel> options,string modelName, IStringLocalizer localizer)
            where TModel : class, new()
        {
            return new CrudAction()
            {
                Visible = options.AllowEditionAction,
                Caption = localizer.GetLocalizedString("GenericCrud.List.Edit.Caption", "Edit the {0}", modelName.ToLower()),
                Text = localizer.GetLocalizedString("GenericCrud.List.Edit.Text", "Edit"),
                IconClass = options?.Actions?.Edit?.IconClass ?? ActionOptions.DefaultEdit.IconClass,
                ButtonClass = options?.Actions?.Edit?.ButtonClass ?? ActionOptions.DefaultEdit.ButtonClass,
            };
        }
        private static CrudAction ConfigureDeleteAction<TModel>(this ControllerOptions<TModel> options,string modelName, IStringLocalizer localizer)
            where TModel : class, new()
        {
            return new CrudAction()
            {
                Visible = options.AllowDeletionAction,
                Caption = localizer.GetLocalizedString("GenericCrud.List.Delete.Caption", "Delete the {0}", modelName.ToLower()),
                Text = localizer.GetLocalizedString("GenericCrud.List.Delete.Text", "Delete"),
                IconClass = options?.Actions?.Delete?.IconClass ?? ActionOptions.DefaultDelete.IconClass,
                ButtonClass = options?.Actions?.Delete?.ButtonClass ?? ActionOptions.DefaultDelete.ButtonClass,
            };
        }
        public static CrudList<TModel> CreateListModel<TModel>(this ControllerOptions<TModel> options, IStringLocalizer localizer)
            where TModel : class, new()
        {
            var modelName = typeof(TModel).Name;
            var result = new CrudList<TModel>()
            {
                Title = modelName,
                Subtitle = localizer.GetLocalizedString("GenericCrud.List.Subtitle", "List"),
                Data = Enumerable.Empty<TModel>(),
                NewAction = options.ConfigureNewAction(modelName, localizer),
                DetailsAction = options.ConfigureDetailsAction(modelName, localizer),
                EditAction = options.ConfigureEditAction(modelName, localizer),
                DeleteAction = options.ConfigureDeleteAction(modelName, localizer),
                KeyPropertyName = options.KeyPropertyName,
                Localizer = localizer
            };
            result.Columns = GetModelColumns(result, localizer);
            return result;
        }
        public static CrudDetails<TModel> CreateDetailsModel<TModel>(this ControllerOptions<TModel> options, IStringLocalizer localizer)
            where TModel : class, new()
        {
            var modelName = typeof(TModel).Name;
            var result = new CrudDetails<TModel>()
            {
                Title = modelName,
                Subtitle = localizer.GetLocalizedString("GenericCrud.Details.Subtitle", "Details"),
                Data = default(TModel),
                EditAction = options.ConfigureEditAction(modelName, localizer),
                DeleteAction = options.ConfigureDeleteAction(modelName, localizer),
                KeyPropertyName = options.KeyPropertyName,
                Localizer = localizer
            };
            result.Columns = GetModelColumns(result, localizer);
            return result;
        }
        /// <summary>
        /// Allows get all properties to show into the Crud list
        /// </summary>
        /// <returns>Collection of the properties names to show</returns>
        public static IEnumerable<string> GetModelColumns<TModel>(this Base<TModel> list, IStringLocalizer localizer)
            where TModel : class, new()
            => GetListProperties(list).Select(p => GetPropertyDisplay(p, localizer));
            
        private static string GetPropertyDisplay(Property p, IStringLocalizer localizer)
        {
            if (p.Display == null)
                return p.Info.Name;
            else {
                var name = p.Display.GetName();
                return string.IsNullOrWhiteSpace(name)? p.Info.Name: localizer[name];
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
        public static IEnumerable<Data> GetModelValues(this ICrudDetails details)
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
            MethodInfo generic = method.MakeGenericMethod(config.GetGenericType());
           return generic.Invoke(null, new object[]{config, true}) as IEnumerable<Property>;            
        }
        /// <summary>
        /// Allows get the properties that shuld appear into the crud list    
        /// </summary>
        /// <param name="config">The configuration of the list</param>
        /// <returns>Collection of properties</returns>
        private static IEnumerable<Property> GetListProperties<TModel>(Base<TModel> config, bool includeNoVisibles = false)
            where TModel : class, new()
        {
            return config.GetGenericType().GetTypeInfo().GetProperties()
                .Select(p => new Property
                {
                    Info = p,
                    List = p.GetCustomAttribute<CrudListAttribute>(),
                    Display = p.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                })
                .Where(p => includeNoVisibles || (p.List?.Visible ?? true))
                .OrderBy(p => p.Display?.Order ?? 0);
        }

    }
}