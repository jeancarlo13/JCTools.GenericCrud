using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Models
{
    public interface IBase
    {        
        string LayoutPage { get; set; }
        string Title { get; set; }
        string Subtitle { get; set; }
        IEnumerable<string> Columns { get; set; }
        string KeyPropertyName { get; set; }
        Type GetGenericType();
        IStringLocalizer Localizer { get; set; }
    }
}