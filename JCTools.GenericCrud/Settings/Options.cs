using System;
using Microsoft.EntityFrameworkCore;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Define the customizable options of the package
    /// </summary>
    public class Options : IOptions
    {
        /// <summary>
        /// The path at the layout page; by default is /Views/Shared/_Layout.cshtml
        /// </summary>
        public virtual string LayoutPath { get; set; } = "/Views/Shared/_Layout.cshtml";

        /// <summary>
        /// True for use modal for the crud actions; False (default) for use separated pages
        /// </summary>
        public virtual bool UseModals { get; set; } = false;
        /// <summary>
        /// True if the creation of new entities is allowed; False if locked
        /// </summary>
        public bool AllowCreationAction { get; set; } = true;
        /// <summary>
        /// True if display the details of the entities is allowed; False if locked
        /// </summary>        
        public bool AllowShowDetailsAction { get; set; } = true;
        /// <summary>
        /// True if the edition of the entities is allowed; False if locked
        /// </summary>
        public bool AllowEditionAction { get; set; } = true;
        /// <summary>
        /// True if the deletion of the entities is allowed; False if locked
        /// </summary>
        public bool AllowDeletionAction { get; set; } = true;

        /// <summary>
        /// Defines the settings to be use for represent/create the CRUD actions
        /// </summary>
        public ActionOptions Actions { get; set; } = new ActionOptions();

        /// <summary>
        /// Function to be invoke for get a new instance of the database context
        /// </summary>
        [Obsolete("Use dependency injection for get the correctly database context.", error: true)]
        public Func<IServiceProvider, DbContext> ContextCreator { get; set; }

        /// <summary>
        /// Allows defined the models to be used for the CRUDs creation
        /// </summary>
        public CrudTypeCollection Models { get; set; } = new CrudTypeCollection();
    }
}