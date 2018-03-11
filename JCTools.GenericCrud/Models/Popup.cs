namespace JCTools.GenericCrud.Models
{
    public class Popup
    {
        public string Title
        {
            get => Model?.Subtitle;
        }
        public CrudAction CommitAction
        {
            get;
            set;
        }

        public IBase Model
        {
            get;
            set;
        }
        public string InnerView
        {
            get;
            set;
        }
    }
}