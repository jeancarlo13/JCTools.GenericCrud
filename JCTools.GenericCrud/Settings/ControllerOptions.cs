using System;
using JCTools.GenericCrud.Helpers;
using JCTools.GenericCrud.Models;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Settings
{
    public class ControllerOptions<TModel, TKey> : Options, IControllerOptions
    where TModel : class, new()
    {
        public ControllerOptions(string keyPropertyName) : base()
        {
            KeyPropertyName = keyPropertyName;
        }
        public ControllerOptions(
            Options options,
            string keyPropertyName,
            IStringLocalizer _localizer
        ) : this(keyPropertyName)
        {
            LayoutPath = options.LayoutPath;
            AllowCreationAction = options.AllowCreationAction;
            AllowShowDetailsAction = options.AllowShowDetailsAction;
            AllowEditionAction = options.AllowEditionAction;
            AllowDeletionAction = options.AllowDeletionAction;
            UseModals = options.UseModals;

            ListOptions = this.CreateListModel(_localizer);
            DetailsOptions = this.CreateDetailsModel(_localizer);
            EditOptions = this.CreateEditModel<TModel, TKey>(_localizer);
            CreateOptions = this.CreateCreateModel<TModel, TKey>(_localizer);
            DeleteOptions = this.CreateDeleteModel(_localizer);
        }

        public ICrudList ListOptions
        {
            get;
            set;
        }
        public ICrudDetails DetailsOptions
        {
            get;
            set;
        }
        public ICrudEdit EditOptions
        {
            get;
            set;
        }
        public ICrudEdit CreateOptions
        {
            get;
            set;
        }
        public ICrudDetails DeleteOptions
        {
            get;
            set;
        }
        private string _keyPropertyName;
        public string KeyPropertyName
        {
            get => _keyPropertyName;
            set
            {
                _keyPropertyName = value;
                SetProperty(nameof(KeyPropertyName), value);
            }
        }
        public override string LayoutPath
        {
            get => base.LayoutPath;
            set
            {
                base.LayoutPath = value;
                SetProperty(nameof(LayoutPath), value);
            }
        }
        public override bool UseModals
        {
            get => base.UseModals;
            set
            {
                base.UseModals = value;
                SetProperty(nameof(UseModals), value);
            }
        }
        private void SetProperty<TValue>(string property, TValue value)
        {
            SetProperty(ListOptions, property, value);
            SetProperty(DetailsOptions, property, value);
            SetProperty(CreateOptions, property, value);
            SetProperty(EditOptions, property, value);
            SetProperty(DeleteOptions, property, value);
        }
        private void SetProperty<TValue>(IBase options, string property, TValue value) => options?.GetType().GetProperty(property)?.SetValue(options, value);

        public Type GetModelType() => typeof(TModel);
    }
}