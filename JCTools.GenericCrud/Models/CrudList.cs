using System;
using System.Collections.Generic;
using System.Linq;

namespace JCTools.GenericCrud.Models
{
    public class CrudList<TModel, TKey> : Base<TModel, TKey>, ICrudList
    where TModel : class, new()
    {
        internal IEnumerable<TModel> Data
        {
            get;
            set;
        }
        public CrudAction NewAction
        {
            get;
            set;
        }
        public CrudAction DetailsAction
        {
            get;
            set;
        }
        public CrudAction EditAction
        {
            get;
            set;
        }
        public CrudAction DeleteAction
        {
            get;
            set;
        }
        public bool ActionsColumnIsVisible
        {
            get => NewAction.Visible || DetailsAction.Visible || EditAction.Visible || DeleteAction.Visible;
        }
        public IEnumerable<object> GetData() => Data;
        public void SetData(IQueryable model) => Data = model as IEnumerable<TModel>;
        public string Message
        {
            get;
            set;
        }
        public string MessageClass
        {
            get;
            set;
        }
    }
}