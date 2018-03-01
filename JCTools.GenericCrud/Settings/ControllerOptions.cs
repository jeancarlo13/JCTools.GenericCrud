using JCTools.GenericCrud.Models;

namespace JCTools.GenericCrud.Settings
{
    public class ControllerOptions<TModel> : Options
        where TModel : class, new () 
    {
        public ControllerOptions(string keyPropertyName) : base()
        {
            KeyPropertyName = keyPropertyName;
        }
        public ControllerOptions(Options options, string keyPropertyName) : this(keyPropertyName)
        {
            LayoutPath = options.LayoutPath;
            AllowCreationAction = options.AllowCreationAction;
            AllowShowDetailsAction = options.AllowShowDetailsAction;
            AllowEditionAction = options.AllowEditionAction;
            AllowDeletionAction = options.AllowDeletionAction;
        }

        public CrudList<TModel> ListOptions { get; set; }
        public CrudDetails<TModel> DetailsOptions { get; set; }
        public CrudEdit<TModel> EditOptions { get; set; }
        public string KeyPropertyName { get; set; }
        
    }
}