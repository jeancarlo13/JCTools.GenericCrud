using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Models
{
    public class Base<TModel, TKey> : IBase
    where TModel : class, new()
    {
        public virtual string LayoutPage
        {
            get;
            set;
        } = Configurator.Options.LayoutPath;
        public virtual string Title
        {
            get;
            set;
        }
        public virtual string Subtitle
        {
            get;
            set;
        }
        public virtual IEnumerable<string> Columns
        {
            get;
            set;
        }
        public virtual Type GetModelGenericType() => typeof(TModel);
        public virtual Type GetKeyGenericType() => typeof(TKey);
        public string KeyPropertyName
        {
            get;
            set;
        }
        public IStringLocalizer Localizer
        {
            get;
            set;
        }

        public TKey Id
        {
            get;
            set;
        }
        public object GetId() => Id;
    }
}