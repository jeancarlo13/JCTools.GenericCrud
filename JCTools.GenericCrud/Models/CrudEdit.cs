namespace JCTools.GenericCrud.Models
{
    public class CrudEdit<TModel>: Base<TModel>, ICrudEdit
        where TModel : class, new () 
    {
        public CrudAction IndexAction { get; set; }
        public CrudAction SaveAction { get; set; }
        internal TModel Data { get; set; }
        public object GetData() => Data;

    }
}