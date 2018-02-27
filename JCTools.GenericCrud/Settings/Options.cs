namespace JCTools.GenericCrud.Settings
{
    public class Options
    {
        public string LayoutPath { get; set; } = "/Views/Shared/_Layout.cshtml";
        public bool AllowCreationAction { get; set; } = true;
        public bool AllowShowDetailsAction { get; set; } = true;
        public bool AllowEditionAction { get; set; } = true;
        public bool AllowDeletionAction { get; set; } = true;
    }
}