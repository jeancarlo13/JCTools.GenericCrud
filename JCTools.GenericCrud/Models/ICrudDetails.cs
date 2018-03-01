namespace JCTools.GenericCrud.Models
{
    public interface ICrudDetails:IBase
    {
        CrudAction IndexAction { get; set; }
        CrudAction EditAction { get; set; }
        CrudAction DeleteAction { get; set; }
        object GetData();

    }
}