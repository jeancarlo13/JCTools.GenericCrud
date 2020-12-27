
using System;
using System.Collections.Generic;
using System.Linq;
using JCTools.GenericCrud.Helpers;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// Contains the data for the creation and manage of the entities into the views
    /// </summary>
    /// <typeparam name="TModel">The type of the model that represents the entities to modified</typeparam>
    /// <typeparam name="TKey">The type of the property identifier of the entity model</typeparam>
    public class CrudModel<TModel, TKey> : IDetailsModel, IEditModel, IIndexModel
        where TModel : class, new()
    {
        /// <summary>
        /// The js function to invoked when the user required a CRUD action and use modals
        /// </summary>
        private const string _onActionClickScript = "genericCrud.showModal.call(this)";

        /// <summary>
        /// The pattern of the localized string to be used for the subtitle property
        /// </summary>
        private const string _subtitleI18NKey = "GenericCrud.{0}.Subtitle";
        /// <summary>
        /// The CRUD type to be used for configure the instance
        /// </summary>
        private readonly ICrudType _crudType;

        /// <summary>        
        /// The entire data of all entities to be displayed into the view
        /// </summary>
        private IEnumerable<TModel> _data;

        /// <summary>
        /// The Id/Key of the related entity to the data to be displayed into the view
        /// </summary>
        private TKey _modelId;

        /// <summary>
        /// The instance of <see cref="IStringLocalizer"/> used for translate 
        /// the texts to displayed into the view
        /// </summary>
        private readonly IStringLocalizer _localizer;
        /// <summary>
        /// The <see cref="IUrlHelper"/> instance to use to generate the action urls
        /// </summary>
        private readonly IUrlHelper _urlHelper;

        /// <summary>
        /// The process where the current instance to be used
        /// </summary>
        public CrudProcesses CurrentProcess { get; set; } = CrudProcesses.None;

        /// <summary>
        /// The path at the layout page; by default is /Views/Shared/_Layout.cshtml
        /// </summary>
        public virtual string LayoutPage { get; set; } = Configurator.Options.LayoutPath;

        /// <summary>
        /// True for use modal for the crud actions; False (default) for use separated pages
        /// </summary>
        /// <remarks>Required Bootstrap v3.3.7 &gt;= version &lt; v4.0.0</remarks>
        public bool UseModals { get; set; } = Configurator.Options.UseModals;

        /// <summary>
        /// The title to display into the view
        /// </summary>
        public virtual string Title { get => GetModelName(); }

        /// <summary>
        /// The subtitle to display into the view
        /// </summary>
        public virtual string Subtitle
        {
            get
            {
                var process = CurrentProcess == CrudProcesses.Save ? CrudProcesses.Edit.ToString()
                    : CurrentProcess != CrudProcesses.None ? CurrentProcess.ToString()
                    : string.Empty;

                return _localizer.GetLocalizedString(string.Format(_subtitleI18NKey, process), process);
            }
        }

        /// <summary>
        /// True if the columns with the CRUD actions (buttons/icons)
        /// are displayed into the list of the Index view;
        /// another, false.
        /// </summary>        
        public bool ShowActionsColumns
        {
            get => NewAction.Visible || DetailsAction.Visible || EditAction.Visible || DeleteAction.Visible;
        }

        /// <summary>
        /// The collection of the column names to be displayed 
        /// into the view
        /// </summary>
        public virtual IEnumerable<string> Columns
        {
            get => _crudType.GetProperties(_localizer)
                .Select(p => p.ToString());
        }


        /// <summary>
        /// The name of the property used how to key/id of the model
        /// </summary>
        public string KeyPropertyName { get => _crudType.KeyPropertyName; }

        /// <summary>
        /// The message to be displayed into the view.
        /// Used for display the success or error messages into the CRUD operations
        /// </summary>
        public ViewMessage Message { get; set; }


        /// <summary>
        /// True if the current CRUD use a custom controller; 
        /// False if use the <see cref="Controllers.GenericController{TContext, TModel, TKey}"/> class
        /// </summary>
        public bool UseCustomController { get => !_crudType.UseGenericController; }

        /// <summary>
        /// The configuration to be used for represents the icon/button 
        /// of the "Go To Index" action
        /// </summary>
        private CrudAction _indexAction;

        /// <summary>
        /// The configuration to be used for represents the icon/button 
        /// of the "Go To Index" action
        /// </summary>
        public CrudAction IndexAction
        {
            get
            {
                if (_indexAction == null)
                    _indexAction = new CrudAction()
                    {
                        Visible = Configurator.Options?.AllowCreationAction ?? true,
                        Caption = _localizer.GetLocalizedString(
                            "GenericCrud.List.Index.Caption",
                            "Go back",
                            GetModelType().Name.ToLower()
                        ),
                        Text = _localizer.GetLocalizedString("GenericCrud.List.Index.Text", "Go back"),
                        IconClass = Configurator.Options?.Actions?.Index?.IconClass
                            ?? ActionOptions.DefaultIndex.IconClass,
                        ButtonClass = Configurator.Options?.Actions?.Index?.ButtonClass
                            ?? ActionOptions.DefaultIndex.ButtonClass,
                        Url = _urlHelper.Action("Index")
                    };

                return _indexAction;
            }

            set => _indexAction = value;
        }

        /// <summary>
        /// The configuration to be used for represents the icon/button 
        /// of the new entity action
        /// </summary>
        private CrudAction _newAction;

        /// <summary>
        /// The configuration to be used for represents the icon/button 
        /// of the new entity action
        /// </summary>
        public CrudAction NewAction
        {
            get
            {
                if (_newAction == null)
                    _newAction = new CrudAction()
                    {
                        Visible = Configurator.Options?.AllowCreationAction ?? true,
                        Caption = _localizer.GetLocalizedString(
                                "GenericCrud.List.Create.Caption",
                                    "Create new {0}",
                                    GetModelType().Name.ToLower()
                            ),
                        Text = _localizer.GetLocalizedString("GenericCrud.List.Create.Text", "Create"),
                        IconClass = Configurator.Options?.Actions?.New?.IconClass
                            ?? ActionOptions.DefaultNew.IconClass,
                        ButtonClass = Configurator.Options?.Actions?.New?.ButtonClass
                            ?? ActionOptions.DefaultNew.ButtonClass,
                        OnClientClick = _onActionClickScript,
                        Url = _urlHelper.Action("Create")
                    };

                return _newAction;
            }

            set => _newAction = value;
        }

        /// <summary>
        /// The configuration to be used for represents the icon/button 
        /// of the save entity action
        /// </summary>
        private CrudAction _saveAction;

        /// <summary>
        /// The configuration to be used for represents the icon/button 
        /// of the save entity action
        /// </summary>
        public CrudAction SaveAction
        {
            get
            {
                if (_saveAction == null)
                    _saveAction = new CrudAction()
                    {
                        Visible = Configurator.Options?.AllowCreationAction ?? true,
                        Caption = _localizer.GetLocalizedString(
                            "GenericCrud.List.Save.Caption",
                            "Save changes",
                            GetModelType().Name.ToLower()
                        ),
                        Text = _localizer.GetLocalizedString("GenericCrud.List.Save.Text", "Save"),
                        IconClass = Configurator.Options?.Actions?.Save?.IconClass
                            ?? ActionOptions.DefaultSave.IconClass,
                        ButtonClass = Configurator.Options?.Actions?.Save?.ButtonClass
                            ?? ActionOptions.DefaultSave.ButtonClass
                    };

                if (CurrentProcess == CrudProcesses.Create)
                    _saveAction.Url = _urlHelper.Action("Save");
                else if (CurrentProcess == CrudProcesses.Edit)
                    _saveAction.Url = _urlHelper.Action("SaveChangesAsync", new { id = _modelId });

                return _saveAction;
            }
            set => _saveAction = value;
        }

        /// <summary>
        /// The configuration to be used for represents the icon/button 
        /// of the save entity action
        /// </summary>
        private CrudAction _detailsAction;

        /// <summary>
        /// The configuration to be used for represents the icon/button 
        /// of the action to be display the entity details
        /// </summary>
        public CrudAction DetailsAction
        {
            get
            {
                if (_detailsAction == null)
                    _detailsAction = new CrudAction()
                    {
                        Visible = Configurator.Options?.AllowShowDetailsAction ?? true,
                        Caption = _localizer.GetLocalizedString(
                            "GenericCrud.List.Details.Caption",
                            "Details of the {0}",
                            GetModelType().Name.ToLower()
                        ),
                        Text = _localizer.GetLocalizedString("GenericCrud.List.Details.Text", "Details"),
                        IconClass = Configurator.Options?.Actions?.Details?.IconClass ?? ActionOptions.DefaultDetails.IconClass,
                        ButtonClass = Configurator.Options?.Actions?.Details?.ButtonClass ?? ActionOptions.DefaultDetails.ButtonClass,
                        OnClientClick = _onActionClickScript
                    };

                return _detailsAction;
            }
            set => _detailsAction = value;
        }


        /// <summary>
        /// The configuration to be used for represents the icon/button 
        /// of the edit action
        /// </summary>
        private CrudAction _editAction;

        /// <summary>
        /// The configuration to be used for represents the icon/button 
        /// of the edit action
        /// </summary>
        public CrudAction EditAction
        {
            get
            {
                if (_editAction == null)
                    _editAction = new CrudAction()
                    {
                        Visible = Configurator.Options?.AllowEditionAction ?? true,
                        Caption = _localizer.GetLocalizedString(
                            "GenericCrud.List.Edit.Caption",
                            "Edit of the {0}",
                            GetModelType().Name.ToLower()
                        ),
                        Text = _localizer.GetLocalizedString("GenericCrud.List.Edit.Text", "Edit"),
                        IconClass = Configurator.Options?.Actions?.Edit?.IconClass
                            ?? ActionOptions.DefaultEdit.IconClass,
                        ButtonClass = Configurator.Options?.Actions?.Edit?.ButtonClass
                            ?? ActionOptions.DefaultEdit.ButtonClass,
                        OnClientClick = _onActionClickScript,
                        UseSubmit = true
                    };

                return _editAction;
            }
            set => _editAction = value;
        }

        /// <summary>
        /// The configuration to be used for represents the icon/button
        /// of the delete entity action
        /// </summary>
        private CrudAction _deleteAction;

        /// <summary>
        /// The configuration to be used for represents the icon/button
        /// of the delete entity action
        /// </summary>
        public CrudAction DeleteAction
        {
            get
            {
                if (_deleteAction == null)
                {
                    _deleteAction = new CrudAction()
                    {
                        Visible = Configurator.Options?.AllowEditionAction ?? true,
                        Caption = _localizer.GetLocalizedString(
                           "GenericCrud.List.Delete.Caption",
                           "Delete of the {0}",
                           GetModelType().Name.ToLower()
                       ),
                        Text = _localizer.GetLocalizedString("GenericCrud.List.Delete.Text", "Delete"),
                        IconClass = Configurator.Options?.Actions?.Delete?.IconClass
                           ?? ActionOptions.DefaultDelete.IconClass,
                        ButtonClass = Configurator.Options?.Actions?.Delete?.ButtonClass
                           ?? ActionOptions.DefaultDelete.ButtonClass,
                        OnClientClick = _onActionClickScript,
                        Url = _urlHelper.Action("DeleteConfirm", new { id = _modelId }),
                    };

                    if (!_deleteAction.UseModals)
                        _deleteAction.UseText = true;
                }

                return _deleteAction;
            }
            set => _deleteAction = value;
        }

        /// <summary>
        /// Allows init the instance with the CRUD type related to the specified model and Key/Id property 
        /// </summary>
        /// <param name="keyPropertyName">The name of the Id/Key property</param>
        /// <param name="localizer">The instance of <see cref="IStringLocalizer"/> used for translate 
        /// the texts to displayed into the view</param>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/> instance to use to generate the action urls</param>
        public CrudModel(string keyPropertyName, IStringLocalizer localizer, IUrlHelper urlHelper)
            : this(Configurator.Options.Models[typeof(TModel), keyPropertyName], localizer, urlHelper)
        { }

        /// <summary>
        /// Allows init the instance with the specified cRUD type 
        /// </summary>
        /// <param name="crud">The CRUD type to be used for configure the instance</param>
        /// <param name="localizer">The instance of <see cref="IStringLocalizer"/> used for translate 
        /// the texts to displayed into the view</param>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/> instance to use to generate the action urls</param>
        internal CrudModel(ICrudType crud, IStringLocalizer localizer, IUrlHelper urlHelper)
        {
            if (crud is null)
                throw new ArgumentNullException(
                    nameof(crud),
                    "Make sure that crud is properly registed into the startup file."
                );

            _localizer = localizer;
            _urlHelper = urlHelper;
            _crudType = crud;

            _data = Enumerable.Empty<TModel>();
        }

        /// <summary>
        /// Allows get the type of the model represented into the views
        /// </summary>
        /// <returns>The found type</returns>
        public virtual Type GetModelType()
            => _crudType.ModelType;

        /// <summary>
        /// Allows get the localized name of the model represented into the views
        /// </summary>
        /// <returns>The found name</returns>
        public virtual string GetModelName()
        {
            var name = _crudType.ModelType.Name;
            return _localizer.GetLocalizedString(name, name);
        }

        /// <summary>
        /// Allows get the type of the model id/key property
        /// </summary>
        /// <returns>The found type</returns>
        public virtual Type GetKeyType()
            => _crudType.KeyPropertyType;

        /// <summary>
        /// Allows get the entire data of the entity to be displayed into the view
        /// </summary>
        /// <returns>The stored data</returns>
        public IEntityData GetData()
        {
            var data = _data?.FirstOrDefault() ?? default(TModel);
            return new EntityData<TModel, TKey>(data, _crudType, _localizer);
        }

        /// <summary>
        /// Allows set/change the entire data of the entity to be displayed into the view
        /// </summary>
        /// <param name="model">The entire data to be set</param>
        public void SetData(object model)
        {
            if (model is TModel instance)
            {
                _data = new List<TModel>() { instance as TModel };
                _modelId = (TKey)_crudType.GetKeyPropertyValue(instance);
            }
            else
                throw new ArgumentException($"The provided object model is not a \"{typeof(TModel)}\" instance.");
        }

        /// <summary>
        /// Allows get the entire data of all entities to be displayed into the view
        /// </summary>
        /// <returns>The stored data</returns>
        public IEnumerable<IEntityData> GetCollectionData()
        {
            var data = _data is IEnumerable<TModel> enumerated
                 ? enumerated
                 : Enumerable.Empty<TModel>();

            return data.Select(d => new EntityData<TModel, TKey>(d, _crudType, _localizer));
        }

        /// <summary>
        /// Sets a collection of entities to be displayed into the view
        /// </summary>
        /// <param name="data">The entities collection to be set</param>
        public void SetData(IEnumerable<object> data)
        {
            _data = data.OfType<TModel>().ToList();
            _modelId = default(TKey);
        }

        /// <summary>
        /// Allows get the value of the Id/Key property of the entity
        /// </summary>
        /// <returns>The found value</returns>
        public object GetId()
            => _modelId;

        /// <summary>
        /// Allows set/change the Id/Key property value of the entity to be displayed into the view
        /// </summary>
        /// <param name="id">The Id/Key property value to be set</param>
        public void SetId(object id) => _modelId = (TKey)id;

        /// <summary>
        /// Allows get the Key/Id property value of the specific instance
        /// </summary>
        /// <param name="obj">The instance to be evaluated</param>
        /// <returns>The found Key/Id property value or null</returns>
        public object GetKeyPropertyValue(object obj)
            => _crudType.GetKeyPropertyValue(obj);

    }
}