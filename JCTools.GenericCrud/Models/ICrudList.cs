using System;
using System.Collections.Generic;

namespace JCTools.GenericCrud.Models
{
    public interface ICrudList
    {
        string LayoutPage { get; set; } 
        string Title { get; set; }
        string Subtitle { get; set; }
        CrudAction NewAction { get; set; }
        CrudAction DetailsAction { get; set; }
        CrudAction EditAction { get; set; }
        CrudAction DeleteAction { get; set; }
        bool ActionsColumnIsVisible { get; }

        Type GetGenericType();
        IEnumerable<Object> GetData();
    }
}