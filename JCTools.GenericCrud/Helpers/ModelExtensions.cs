using System;
using JCTools.GenericCrud.Attibutes;
using JCTools.GenericCrud.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JCTools.GenericCrud.Helpers
{
    /// <summary>
    /// Extensors used for get the model properties
    /// </summary>
    public static class ModelExtensions
    {
        public static CrudList<TModel> CreateListModel<TModel>(this DbSet<TModel> data, Options options)
        where TModel : class, new()
        {
            var modelName = typeof(TModel).Name;
            return new CrudList<TModel>()
            {
                Title = modelName,
                    Subtitle = "List",
                    Data = data,
                    NewAction = new CrudAction()
                    {
                        Visible = options.AllowCreationAction,
                        Caption = $"Create new {modelName.ToLower()}",
                        Text = "Create",
                        IconClass = "fa fa-plus"
                    },
                    DetailsAction = new CrudAction()
                    {
                        Visible = options.AllowShowDetailsAction,
                        Caption = $"Details of the {modelName.ToLower()}",
                        Text = "Details",
                        IconClass = "fa fa-info-circle"
                    },
                    EditAction = new CrudAction()
                    {
                        Visible = options.AllowEditionAction,
                        Caption = $"Edit {modelName.ToLower()}",
                        Text = "Edit",
                        IconClass = "fa fa-pencil-alt"
                    },
                    DeleteAction = new CrudAction()
                    {
                        Visible = options.AllowDeletionAction,
                        Caption = $"Delete the {modelName.ToLower()}",
                        Text = "Delete",
                        IconClass = "fa fa-trash"
                    }
            };
        }
        /// <summary>
        /// Allows get all properties to show into the Crud list
        /// </summary>
        /// <returns>Collection of the properties names to show</returns>
        public static IEnumerable<string> GetModelColumns(this ICrudList list)
         => GetListProperties(list).Select(p => p.Display?.GetName() ?? p.Info.Name);
        public static IEnumerable<IEnumerable<object>> GetModelValues(this ICrudList list)
        {
            var properties = GetListProperties(list);
            return list.GetData()
                .Select(d => properties.Select(p => p.Info.GetValue(d)));
        }
        /// <summary>
        /// Allows get the properties that shuld appear into the crud list    
        /// </summary>
        /// <param name="list">The configuration of the list</param>
        /// <returns>Collection of properties</returns>
        private static IEnumerable<Property> GetListProperties(ICrudList list)
        {
            return list.GetGenericType().GetTypeInfo().GetProperties()
                .Select(p => new Property
                {
                    Info = p,
                    List = p.GetCustomAttribute<CrudListAttribute>(),
                    Display = p.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                })
                .Where(p => p.List?.Visible ?? true)
                .OrderBy(p => p.List?.Order ?? 0);
        }
    }
}