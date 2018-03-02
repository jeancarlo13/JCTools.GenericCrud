using JCTools.GenericCrud.Models;

namespace JCTools.GenericCrud.Settings
{
    public class ControllerOptions<TModel, TKey> : Options
    where TModel : class, new()
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

        public CrudList<TModel, TKey> ListOptions
        {
            get;
            set;
        }
        public CrudDetails<TModel, TKey> DetailsOptions
        {
            get;
            set;
        }
        public CrudEdit<TModel, TKey> EditOptions
        {
            get;
            set;
        }
        public string KeyPropertyName
        {
            get;
            set;
        }

    }
}