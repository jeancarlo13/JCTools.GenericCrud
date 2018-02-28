using JCTools.GenericCrud.Settings;

namespace JCTools.GenericCrud.Models
{
    public class CrudAction: CrudActionBase
    {
        public bool Visible { get; set; }
        public string Caption { get; set; }
        public string Text { get; set; }
    }
}