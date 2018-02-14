using System;
using System.Collections.Generic;

namespace JCTools.GenericCrud.Models {
    public class CrudList<TModel> : ICrudList
        where TModel : class, new () 
    {
        public string LayoutPage { get; set; } = Configurator.Options.LayoutPath;
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public IEnumerable<TModel> Data { get; set; }
        public CrudAction NewAction { get; set; }
        public CrudAction DetailsAction { get; set; }
        public CrudAction EditAction { get; set; }
        public CrudAction DeleteAction { get; set; }
        public bool ActionsColumnIsVisible { get => NewAction.Visible || DetailsAction.Visible || EditAction.Visible || DeleteAction.Visible; }

        public Type GetGenericType() => typeof(TModel);
        public IEnumerable<object> GetData() => Data;
    }
}