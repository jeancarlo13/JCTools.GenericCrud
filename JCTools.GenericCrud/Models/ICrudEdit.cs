namespace JCTools.GenericCrud.Models
{
    public interface ICrudEdit : IBaseDetails
    {
        CrudAction SaveAction { get; set; }
    }
}