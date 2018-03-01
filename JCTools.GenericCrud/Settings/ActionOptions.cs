namespace JCTools.GenericCrud.Settings
{
    public class ActionOptions{
        internal static readonly CrudActionBase DefaultIndex  
            = new CrudActionBase{ ButtonClass = "btn btn-default btn-sm" };
        internal static readonly CrudActionBase DefaultNew  
            = new CrudActionBase{ IconClass = "fa fa-plus", ButtonClass = "btn btn-default btn-sm" };
        internal static readonly CrudActionBase DefaultDetails  
            = new CrudActionBase{ IconClass = "fa fa-info-circle", ButtonClass = "btn btn-default btn-sm" };
        internal static readonly CrudActionBase DefaultEdit  
            = new CrudActionBase{ IconClass = "fa fa-pencil-alt", ButtonClass = "btn btn-default btn-sm" };
        internal static readonly CrudActionBase DefaultDelete  
            = new CrudActionBase{ IconClass = "fa fa-trash", ButtonClass = "btn btn-danger btn-sm" };
        internal static readonly CrudActionBase DefaultSave  
            = new CrudActionBase{  ButtonClass = "btn btn-primary btn-sm" };
            

        public CrudActionBase Index { get; set; } = DefaultIndex;
        public CrudActionBase New { get; set; } = DefaultNew;
        public CrudActionBase Details { get; set; } = DefaultDetails;
        public CrudActionBase Edit { get; set; } = DefaultEdit;
        public CrudActionBase Delete { get; set; } = DefaultDelete;
        public CrudActionBase Save { get; set; } = DefaultSave;
    }
}