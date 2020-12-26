namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// Define the information required for display the entities to the user
    /// </summary>
    public class Popup
    {
        /// <summary>
        /// The action to be invoke for save the user changes
        /// </summary>
        private CrudAction _commitAction;

        /// <summary>
        /// The title of the modal to display
        /// </summary>
        public string Title { get => Model?.Subtitle; }
        /// <summary>
        /// The action to be invoke for save the user changes
        /// </summary>
        public CrudAction CommitAction
        {
            get => _commitAction;
            set
            {
                _commitAction = value;
                if (_commitAction != null)
                {
                    _commitAction.UseModals = true;
                    _commitAction.UseText = true;
                    const string onExecuteCommitScript = "genericCrud.executeCommitAction.call(this)";
                    _commitAction.OnClientClick = onExecuteCommitScript;
                }
            }
        }
        /// <summary>
        /// The entity to be displayed to the user
        /// </summary>
        public IViewModel Model { get; set; }
        /// <summary>
        /// The view to be used for display the entity data to the user
        /// </summary>
        public string InnerView { get; set; }
    }
}