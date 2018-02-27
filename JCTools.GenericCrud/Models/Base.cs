using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Models
{
    public class Base<TModel>: IBase
        where TModel : class, new () 
    {        
        public virtual string LayoutPage { get; set; } = Configurator.Options.LayoutPath;
        public virtual string Title { get; set; }
        public virtual string Subtitle { get; set; }
        public virtual IEnumerable<string> Columns { get; set; }
        public virtual Type GetGenericType() => typeof(TModel);
        public string KeyPropertyName { get; set; }
        public IStringLocalizer Localizer { get; set; }
    }
}