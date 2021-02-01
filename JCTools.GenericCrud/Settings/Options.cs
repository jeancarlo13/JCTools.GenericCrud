using System;
using System.Resources;
using JCTools.GenericCrud.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Define the customizable options of the package
    /// </summary>
    internal class Options : IOptions
    {
        /// <summary>
        /// The path at the layout page; by default is /Views/Shared/_Layout.cshtml
        /// </summary>
        public virtual string LayoutPath { get; set; } = "/Views/Shared/_Layout.cshtml";

        /// <summary>
        /// True for use modal for the crud actions; False (default) for use separated pages
        /// </summary>
        /// <remarks>Required Bootstrap v3.3.7 &gt;= version &lt; v4.0.0</remarks>
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

        /// <summary>
        /// Allows set the version of bootstrap to be used;
        /// Default <see cref="Bootstrap.Version4"/>
        /// </summary>
        public Bootstrap BootstrapVersion { get; set; } = Bootstrap.Version4;

        /// <summary>
        /// The cached <see cref="ResourceManager"/> instance to be used
        /// </summary>
        public ResourceManager ResourceManager { get; private set; } = Resources.I18N.ResourceManager;

        /// <summary>
        /// The action to invoke to configure the authorization policy used to manage access to CRUD controllers
        /// </summary>
        /// <remarks>By default, only is required an authenticated user</remarks>
        public Action<AuthorizationPolicyBuilder> AuthorizationPolicy { get; private set; } = null;

        /// <summary>
        /// Allows replace the default localization strings for the CRUDs
        /// </summary>
        /// <param name="resourceManager">The cached <see cref="ResourceManager"/> instance to be used</param>
        public void ReplaceLocalization(ResourceManager resourceManager)
            => ResourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));

        /// <summary>
        /// It allows to indicate if it is required to enable the authorization policy 
        /// to grant access to the CRUD controllers
        /// </summary>
        /// <param name="policyFactory">The action builder of the policy to be used. 
        /// If is null, only one authenticated user is required.</param>
        public void UseAuthorization(Action<AuthorizationPolicyBuilder> policyFactory = null)
            => AuthorizationPolicy = policyFactory ?? ((b) => b.RequireAuthenticatedUser());

    }
}