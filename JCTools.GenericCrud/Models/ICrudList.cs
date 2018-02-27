using System;
using System.Collections.Generic;

namespace JCTools.GenericCrud.Models
{
    public interface ICrudList: IBase
    {
        CrudAction NewAction { get; set; }
        CrudAction DetailsAction { get; set; }
        CrudAction EditAction { get; set; }
        CrudAction DeleteAction { get; set; }
        bool ActionsColumnIsVisible { get; }
        IEnumerable<Object> GetData();
    }
}