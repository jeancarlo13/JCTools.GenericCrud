namespace JCTools.GenericCrud.Models
{
    public class CrudAction : BaseAction
    {
        /// <summary>
        /// True if the action button/icon is visible for the user;
        /// Another, false
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// The caption text of the action button/icon
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// The text to displayed in the action button; 
        /// Ignored if used an icon for represent the action
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// True if desired use a submit button for represent the action
        /// </summary>
        public bool UseSubmit { get; set; } = false;
        /// <summary>
        /// True if the related CRUD use bootstrap modals
        /// </summary>
        public bool UseModals { get; set; } = false;
        /// <summary>
        /// True if is desired that use text for represent the action;
        /// Another, false
        /// </summary>
        public bool UseText { get; set; } = false;
        /// <summary>
        /// The url to invoke when the user press the action
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// The js function to invoke when the user press the action
        /// </summary>
        public string OnClientClick { get; set; }
    }
}