using System;

namespace JCTools.GenericCrud.Settings
{
    public interface IOptions
    {
        string LayoutPath
        {
            get;
            set;
        }
        bool UseModals
        {
            get;
            set;
        }
        bool AllowCreationAction
        {
            get;
            set;
        }
        bool AllowShowDetailsAction
        {
            get;
            set;
        }
        bool AllowEditionAction
        {
            get;
            set;
        }
        bool AllowDeletionAction
        {
            get;
            set;
        }
        ActionOptions Actions
        {
            get;
            set;
        }

    }
}