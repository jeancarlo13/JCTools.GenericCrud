using JCTools.GenericCrud.Settings;

namespace JCTools.GenericCrud.Models
{
    public class CrudAction : BaseAction
    {
        public bool Visible { get; set; }
        public string Caption { get; set; }
        public string Text { get; set; }
        public bool UseSubmit { get; set; } = false;

        public bool UseModals { get; set; } = false;

        public bool UseText { get; set; } = false;

        public string Url { get; set; }
        public string OnClientClick { get; set; }
    }
}