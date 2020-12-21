using System;

namespace JCTools.GenericCrud.Settings
{
    public interface IOptions
    {
        /// <summary>
        /// The path at the layout page
        /// </summary>
        string LayoutPath { get; set; }
        /// <summary>
        /// True for use modal for the crud actions; False (default) for use separated pages
        /// </summary>
        bool UseModals { get; set; }
        /// <summary>
        /// True if the creation of new entities is allowed; False if locked
        /// </summary>
        bool AllowCreationAction { get; set; }
        /// <summary>
        /// True if display the details of the entities is allowed; False if locked
        /// </summary>        
        bool AllowShowDetailsAction { get; set; }
        /// <summary>
        /// True if the edition of the entities is allowed; False if locked
        /// </summary>
        bool AllowEditionAction { get; set; }
        /// <summary>
        /// True if the deletion of the entities is allowed; False if locked
        /// </summary>
        bool AllowDeletionAction { get; set; }
        ActionOptions Actions { get; set; }

    }
}