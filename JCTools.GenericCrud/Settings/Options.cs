using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JCTools.GenericCrud.Settings
{
    public class Options : IOptions
    {
        /// <summary>
        /// The path at the layput page; by default is /Views/Shared/_Layout.cshtml
        /// </summary>
        public virtual string LayoutPath
        {
            get;
            set;
        } = "/Views/Shared/_Layout.cshtml";
        /// <summary>
        /// True for use modal for the crud actions; False (default) for use separated pages
        /// </summary>
        /// <returns></returns>
        public virtual bool UseModals
        {
            get;
            set;
        } = false;
        public bool AllowCreationAction
        {
            get;
            set;
        } = true;
        public bool AllowShowDetailsAction
        {
            get;
            set;
        } = true;
        public bool AllowEditionAction
        {
            get;
            set;
        } = true;
        public bool AllowDeletionAction
        {
            get;
            set;
        } = true;
        public ActionOptions Actions
        {
            get;
            set;
        } = new ActionOptions();

        public Func<DbContext> ContextCreator
        {
            get;
            set;
        }

        public CrudTypeCollection Models
        {
            get;
            set;
        } = new CrudTypeCollection();
    }
}