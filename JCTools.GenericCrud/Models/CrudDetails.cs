namespace JCTools.GenericCrud.Models
{
    public class CrudDetails<TModel>: Base<TModel>, ICrudDetails
        where TModel : class, new () 
    {
        public CrudAction EditAction { get; set; }
        public CrudAction DeleteAction { get; set; }
        internal TModel Data { get; set; }
        public object GetData() => Data;

    }
}