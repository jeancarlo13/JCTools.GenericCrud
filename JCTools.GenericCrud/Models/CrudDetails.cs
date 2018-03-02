namespace JCTools.GenericCrud.Models
{
    public class CrudDetails<TModel, TKey> : Base<TModel, TKey>, ICrudDetails
    where TModel : class, new()
    {
        public CrudAction IndexAction
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
        internal TModel Data
        {
            get;
            set;
        }
        public object GetData() => Data;

    }
}