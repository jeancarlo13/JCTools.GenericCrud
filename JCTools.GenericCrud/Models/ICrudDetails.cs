namespace JCTools.GenericCrud.Models
{
    public interface ICrudDetails : IBaseDetails
    {
        CrudAction EditAction { get; set; }
        CrudAction DeleteAction { get; set; }
    }
}