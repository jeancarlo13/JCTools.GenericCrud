using System.Linq;
using JCTools.GenericCrud.Models;
using JCTools.GenericCrud.Settings;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Helpers
{
    public static class SettingsHelper
    {
        private static CrudAction ConfigureNewAction<TModel>(this ControllerOptions<TModel> options,string modelName, IStringLocalizer localizer)
            where TModel : class, new()
        {
            return new CrudAction()
            {
                Visible = options.AllowCreationAction,
                Caption = localizer.GetLocalizedString("GenericCrud.List.Create.Caption", "Create new {0}", modelName.ToLower()),
                Text = localizer.GetLocalizedString("GenericCrud.List.Create.Text", "Create"),
                IconClass = options?.Actions?.New?.IconClass ?? ActionOptions.DefaultNew.IconClass,
                ButtonClass = options?.Actions?.New?.ButtonClass ?? ActionOptions.DefaultNew.ButtonClass,
            };
        }
        private static CrudAction ConfigureSaveAction<TModel>(this ControllerOptions<TModel> options,string modelName, IStringLocalizer localizer)
            where TModel : class, new()
        {
            return new CrudAction()
            {
                Visible = options.AllowCreationAction,
                Caption = localizer.GetLocalizedString("GenericCrud.List.Save.Caption", "Save changes", modelName.ToLower()),
                Text = localizer.GetLocalizedString("GenericCrud.List.Save.Text", "Save"),
                IconClass = options?.Actions?.Save?.IconClass ?? ActionOptions.DefaultSave.IconClass,
                ButtonClass = options?.Actions?.Save?.ButtonClass ?? ActionOptions.DefaultSave.ButtonClass,
            };
        }
        private static CrudAction ConfigureDetailsAction<TModel>(this ControllerOptions<TModel> options,string modelName, IStringLocalizer localizer)
            where TModel : class, new()
        {
            return new CrudAction()
            {
                Visible = options.AllowShowDetailsAction,
                Caption = localizer.GetLocalizedString("GenericCrud.List.Details.Caption", "Details of the {0}", modelName.ToLower()),
                Text = localizer.GetLocalizedString("GenericCrud.List.Details.Text", "Details"),
                IconClass = options?.Actions?.Details?.IconClass ?? ActionOptions.DefaultDetails.IconClass,
                ButtonClass = options?.Actions?.Details?.ButtonClass ?? ActionOptions.DefaultDetails.ButtonClass,
            };
        }
        private static CrudAction ConfigureEditAction<TModel>(this ControllerOptions<TModel> options,string modelName, IStringLocalizer localizer)
            where TModel : class, new()
        {
            return new CrudAction()
            {
                Visible = options.AllowEditionAction,
                Caption = localizer.GetLocalizedString("GenericCrud.List.Edit.Caption", "Edit the {0}", modelName.ToLower()),
                Text = localizer.GetLocalizedString("GenericCrud.List.Edit.Text", "Edit"),
                IconClass = options?.Actions?.Edit?.IconClass ?? ActionOptions.DefaultEdit.IconClass,
                ButtonClass = options?.Actions?.Edit?.ButtonClass ?? ActionOptions.DefaultEdit.ButtonClass,
            };
        }
        private static CrudAction ConfigureDeleteAction<TModel>(this ControllerOptions<TModel> options,string modelName, IStringLocalizer localizer)
            where TModel : class, new()
        {
            return new CrudAction()
            {
                Visible = options.AllowDeletionAction,
                Caption = localizer.GetLocalizedString("GenericCrud.List.Delete.Caption", "Delete the {0}", modelName.ToLower()),
                Text = localizer.GetLocalizedString("GenericCrud.List.Delete.Text", "Delete"),
                IconClass = options?.Actions?.Delete?.IconClass ?? ActionOptions.DefaultDelete.IconClass,
                ButtonClass = options?.Actions?.Delete?.ButtonClass ?? ActionOptions.DefaultDelete.ButtonClass,
            };
        }
        private static CrudAction ConfigureIndexAction<TModel>(this ControllerOptions<TModel> options,string modelName, IStringLocalizer localizer)
            where TModel : class, new()
        {
            return new CrudAction()
            {
                Visible = options.AllowDeletionAction,
                Caption = localizer.GetLocalizedString("GenericCrud.List.Index.Caption", "Go back", modelName.ToLower()),
                Text = localizer.GetLocalizedString("GenericCrud.List.Index.Text", "Go back"),
                IconClass = options?.Actions?.Index?.IconClass ?? ActionOptions.DefaultIndex.IconClass,
                ButtonClass = options?.Actions?.Index?.ButtonClass ?? ActionOptions.DefaultIndex.ButtonClass,
            };
        }
        public static CrudList<TModel> CreateListModel<TModel>(this ControllerOptions<TModel> options, IStringLocalizer localizer)
            where TModel : class, new()
        {
            var modelName = typeof(TModel).Name;
            var result = new CrudList<TModel>()
            {
                Title = modelName,
                Subtitle = localizer.GetLocalizedString("GenericCrud.List.Subtitle", "List"),
                Data = Enumerable.Empty<TModel>(),
                NewAction = options.ConfigureNewAction(modelName, localizer),
                DetailsAction = options.ConfigureDetailsAction(modelName, localizer),
                EditAction = options.ConfigureEditAction(modelName, localizer),
                DeleteAction = options.ConfigureDeleteAction(modelName, localizer),
                KeyPropertyName = options.KeyPropertyName,
                Localizer = localizer
            };
            result.Columns = result.GetModelColumns(localizer);
            return result;
        }
        public static CrudDetails<TModel> CreateDetailsModel<TModel>(this ControllerOptions<TModel> options, IStringLocalizer localizer)
            where TModel : class, new()
        {
            var modelName = typeof(TModel).Name;
            var result = new CrudDetails<TModel>()
            {
                Title = modelName,
                Subtitle = localizer.GetLocalizedString("GenericCrud.Details.Subtitle", "Details"),
                Data = default(TModel),
                IndexAction = options.ConfigureIndexAction(modelName, localizer),
                EditAction = options.ConfigureEditAction(modelName, localizer),
                DeleteAction = options.ConfigureDeleteAction(modelName, localizer),
                KeyPropertyName = options.KeyPropertyName,
                Localizer = localizer
            };
            result.Columns = result.GetModelColumns(localizer);
            return result;
        }
        public static CrudEdit<TModel> CreateEditModel<TModel>(this ControllerOptions<TModel> options, IStringLocalizer localizer)
            where TModel : class, new()
        {
            var modelName = typeof(TModel).Name;
            var result = new CrudEdit<TModel>()
            {
                Title = modelName,
                Subtitle = localizer.GetLocalizedString("GenericCrud.Edit.Subtitle", "Edit"),
                Data = default(TModel),
                IndexAction = options.ConfigureIndexAction(modelName, localizer),
                SaveAction = options.ConfigureSaveAction(modelName, localizer),
                KeyPropertyName = options.KeyPropertyName,
                Localizer = localizer
            };
            result.Columns = result.GetModelColumns(localizer);
            return result;
        }
        
    }
}