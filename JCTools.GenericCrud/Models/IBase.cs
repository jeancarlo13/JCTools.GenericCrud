using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Models
{
    public interface IBase
    {        
        string LayoutPage { get; set; }
        bool UseModals { get; set; }
        string Title { get; set; }
        string Subtitle { get; set; }
        IEnumerable<string> Columns { get; set; }
        string KeyPropertyName { get; set; }
        Type GetModelGenericType();
        Type GetKeyGenericType();
        IStringLocalizer Localizer { get; set; }

        object GetId();
        void SetId(object id);

    }
}