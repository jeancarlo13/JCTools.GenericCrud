namespace JCTools.GenericCrud.Models
{
    public interface ICrudDetails:IBase
    {
        CrudAction EditAction { get; set; }
        CrudAction DeleteAction { get; set; }
        object GetData();

    }
}