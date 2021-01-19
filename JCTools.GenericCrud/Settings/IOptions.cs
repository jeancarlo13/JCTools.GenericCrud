using System;
using System.Resources;
using JCTools.GenericCrud.Models;
using Microsoft.AspNetCore.Authorization;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Define the customizable options of the package
    /// </summary>
    public interface IOptions
    {
        /// <summary>
        /// The path at the layout page
        /// </summary>
        string LayoutPath { get; set; }
        /// <summary>
        /// True for use modal for the crud actions; False (default) for use separated pages
        /// </summary>
        /// <remarks>Required Bootstrap v3.3.7 &gt;= version &lt; v4.0.0</remarks>
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
        /// <summary>
        /// Defines the settings to be use for represent/create the CRUD actions
        /// </summary>
        ActionOptions Actions { get; set; }

        /// <summary>
        /// Allows defined the models to be used for the CRUDs creation
        /// </summary>
        CrudTypeCollection Models { get; set; }

        /// <summary>
        /// Allows set the version of bootstrap to be used;
        /// Default <see cref="Bootstrap.Version4"/>
        /// </summary>
        Bootstrap BootstrapVersion { get; set; }

        /// <summary>
        /// Allows replace the default localization strings for the CRUDs
        /// </summary>
        /// <param name="resourceManager">The cached <see cref="ResourceManager"/> instance to be used</param>
        void ReplaceLocalization(ResourceManager resourceManager);

        /// <summary>
        /// It allows to indicate if it is required to enable the authorization policy 
        /// to grant access to the CRUD controllers
        /// </summary>
        /// <param name="policyFactory">The action builder of the policy to be used. 
        /// If is null, only one authenticated user is required.</param>
        void UseAuthorization(Action<AuthorizationPolicyBuilder> policyFactory = null);
    }
}