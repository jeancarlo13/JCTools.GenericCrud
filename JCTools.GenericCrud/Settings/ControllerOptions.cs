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
        public CrudEdit<TModel, TKey> CreateOptions
        {
            get;
            set;
        }
        public CrudDetails<TModel, TKey> DeleteOptions
        {
            get;
            internal set;
        }
        private string _keyPropertyName;
        public string KeyPropertyName
        {
            get => _keyPropertyName;
            set
            {
                _keyPropertyName = value;
                SetProperty(nameof(KeyPropertyName), value);
            }
        }
        public override string LayoutPath
        {
            get => base.LayoutPath;
            set
            {
                base.LayoutPath = value;
                SetProperty(nameof(LayoutPath), value);
            }
        }
        public override bool UsePopups
        {
            get => base.UsePopups;
            set
            {
                base.UsePopups = value;
                SetProperty(nameof(UsePopups), value);
            }
        }
        private void SetProperty<TValue>(string property, TValue value)
        {
            SetProperty(ListOptions, property, value);
            SetProperty(DetailsOptions, property, value);
            SetProperty(CreateOptions, property, value);
            SetProperty(EditOptions, property, value);
            SetProperty(DeleteOptions, property, value);
        }
        private void SetProperty<TValue>(IBase options, string property, TValue value) => options?.GetType().GetProperty(property)?.SetValue(options, value);
    }
}