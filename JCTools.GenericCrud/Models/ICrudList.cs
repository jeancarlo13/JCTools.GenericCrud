using System;
using System.Collections.Generic;

namespace JCTools.GenericCrud.Models
{
    public interface ICrudList : IBase
    {
        CrudAction NewAction
        {
            get;
        }
        CrudAction DetailsAction
        {
            get;
        }
        CrudAction EditAction
        {
            get;
        }
        CrudAction DeleteAction
        {
            get;
        }
        bool ActionsColumnIsVisible
        {
            get;
        }
        IEnumerable<Object> GetData();
        string Message
        {
            get;
        }
        string MessageClass
        {
            get;
        }
    }
}