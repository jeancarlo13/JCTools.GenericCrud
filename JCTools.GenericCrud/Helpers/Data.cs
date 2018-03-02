namespace JCTools.GenericCrud.Helpers
{
    public class Data
    {
        public string Name
        {
            get;
            set;
        }
        public string Display
        {
            get;
            set;
        }
        public object Value
        {
            get;
            set;
        }
        public bool Visible
        {
            get;
            set;
        }

        public override string ToString() => Name;
    }
}