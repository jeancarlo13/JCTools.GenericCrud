namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Defines the settings to be used for represent/create all actions of the CRUD
    /// </summary>
    public class ActionOptions
    {
        /// <summary>
        /// The default settings for the CRUD actions
        /// </summary>
        internal static readonly BaseAction DefaultIndex
            = new BaseAction
            {
                ButtonClass = "btn btn-default btn-sm"
            };

        /// <summary>
        /// The default settings for the new item action
        /// </summary>
        internal static readonly BaseAction DefaultNew
            = new BaseAction
            {
                IconClass = "fa fa-plus",
                ButtonClass = "btn btn-default btn-sm"
            };

        /// <summary>
        /// The default settings for the show the item's details action
        /// </summary>
        internal static readonly BaseAction DefaultDetails
            = new BaseAction
            {
                IconClass = "fa fa-info-circle",
                ButtonClass = "btn btn-default btn-sm"
            };

        /// <summary>
        /// The default settings for the edit item action
        /// </summary>
        internal static readonly BaseAction DefaultEdit
            = new BaseAction
            {
                IconClass = "fa fa-pencil-alt",
                ButtonClass = "btn btn-default btn-sm"
            };

        /// <summary>
        /// The default settings for the delete item action
        /// </summary>
        internal static readonly BaseAction DefaultDelete
            = new BaseAction
            {
                IconClass = "fa fa-trash",
                ButtonClass = "btn btn-danger btn-sm"
            };

        /// <summary>
        /// The default settings for the save action of the item's changes
        /// </summary>
        internal static readonly BaseAction DefaultSave
            = new BaseAction
            {
                ButtonClass = "btn btn-primary btn-sm"
            };

        /// <summary>
        /// The settings to be used for represent/create the "Go To Index" action
        /// </summary>
        public BaseAction Index { get; set; } = DefaultIndex;
        /// <summary>
        /// The settings to be used for represent/create the new item action
        /// </summary>
        public BaseAction New { get; set; } = DefaultNew;
        /// <summary>
        /// The settings to be used for represent/create the action for display the item's details
        /// </summary>
        public BaseAction Details { get; set; } = DefaultDetails;
        /// <summary>
        /// The settings to be used for represent/create the edit item action
        /// </summary>
        public BaseAction Edit { get; set; } = DefaultEdit;
        /// <summary>
        /// The settings to be used for represent/create the delete item action
        /// </summary>
        public BaseAction Delete { get; set; } = DefaultDelete;
        /// <summary>
        /// The settings to be used for represent/create the save action of the item's changes
        /// </summary>
        public BaseAction Save { get; set; } = DefaultSave;
    }
}