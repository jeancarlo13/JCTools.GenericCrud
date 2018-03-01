namespace JCTools.GenericCrud.Models
{
    public interface IBaseDetails : IBase
    {
        CrudAction IndexAction { get; set; }
        object GetData();
        
    }
}