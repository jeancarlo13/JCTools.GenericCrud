namespace JCTools.GenericCrud.Settings
{
    public class Options
    {
        public virtual string LayoutPath { get; set; } = "/Views/Shared/_Layout.cshtml";
        public virtual bool UsePopups { get; set; } = false;
        public bool AllowCreationAction { get; set; } = true;
        public bool AllowShowDetailsAction { get; set; } = true;
        public bool AllowEditionAction { get; set; } = true;
        public bool AllowDeletionAction { get; set; } = true;
        public ActionOptions Actions { get; set; } = new ActionOptions();
    }
}