using System;
using System.Collections.Generic;
using System.Linq;

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
        void SetData(IQueryable model);
        string Message
        {
            get; set;
        }
        string MessageClass
        {
            get; set;
        }
    }
}