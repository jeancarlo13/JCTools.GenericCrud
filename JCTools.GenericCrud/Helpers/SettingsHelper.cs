using System.Linq;
using JCTools.GenericCrud.Models;
using JCTools.GenericCrud.Settings;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Helpers
{
    public static class SettingsHelper
    {
        private static CrudAction ConfigureNewAction<TModel, TKey>(this ControllerOptions<TModel, TKey> options, string modelName, IStringLocalizer localizer)
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
        private static CrudAction ConfigureSaveAction<TModel, TKey>(this ControllerOptions<TModel, TKey> options, string modelName, IStringLocalizer localizer)
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
        private static CrudAction ConfigureDetailsAction<TModel, TKey>(this ControllerOptions<TModel, TKey> options, string modelName, IStringLocalizer localizer)
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
        private static CrudAction ConfigureEditAction<TModel, TKey>(this ControllerOptions<TModel, TKey> options, string modelName, IStringLocalizer localizer)
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
        public static CrudAction ConfigureDeleteAction<TModel, TKey>(this ControllerOptions<TModel, TKey> options, string modelName, IStringLocalizer localizer)
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
        private static CrudAction ConfigureIndexAction<TModel, TKey>(this ControllerOptions<TModel, TKey> options, string modelName, IStringLocalizer localizer)
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
        public static string GetModelName<TModel, TKey>(this ControllerOptions<TModel, TKey> options, IStringLocalizer localizer)
        where TModel : class, new() 
        => localizer.GetLocalizedString(typeof(TModel).Name, typeof(TModel).Name);
        public static CrudList<TModel, TKey> CreateListModel<TModel, TKey>(this ControllerOptions<TModel, TKey> options, IStringLocalizer localizer)
        where TModel : class, new()
        {
            var modelName = options.GetModelName(localizer);
            var result = new CrudList<TModel, TKey>()
                {
                    Title = modelName,
                    Subtitle = localizer.GetLocalizedString("GenericCrud.List.Subtitle", "List"),
                    Data = Enumerable.Empty<TModel>(),
                    NewAction = options.ConfigureNewAction(modelName, localizer),
                    DetailsAction = options.ConfigureDetailsAction(modelName, localizer),
                    EditAction = options.ConfigureEditAction(modelName, localizer),
                    DeleteAction = options.ConfigureDeleteAction(modelName, localizer),
                    KeyPropertyName = options.KeyPropertyName,
                    Localizer = localizer,
                    UsePopups = options.UsePopups
                };
            result.Columns = result.GetModelColumns(localizer);
            return result;
        }
        public static CrudDetails<TModel, TKey> CreateDetailsModel<TModel, TKey>(this ControllerOptions<TModel, TKey> options, IStringLocalizer localizer)
        where TModel : class, new() => CreateDetailsModel(options, localizer, "GenericCrud.Details.Subtitle", "Details");
        public static CrudDetails<TModel, TKey> CreateDeleteModel<TModel, TKey>(this ControllerOptions<TModel, TKey> options, IStringLocalizer localizer)
        where TModel : class, new()
        {
            var result = CreateDetailsModel(options, localizer, "GenericCrud.Delete.Subtitle", "Are you sure you want to delete this?");
            if (result.DeleteAction.IconClass == ActionOptions.DefaultDelete.IconClass)
                result.DeleteAction.IconClass = string.Empty;
            return result;
        }

        private static CrudDetails<TModel, TKey> CreateDetailsModel<TModel, TKey>(
            this ControllerOptions<TModel, TKey> options,
            IStringLocalizer localizer,
            string subtitleKey,
            string defaultSubtitle)
        where TModel : class, new()
        {
            var modelName = options.GetModelName(localizer);
            var result = new CrudDetails<TModel, TKey>()
                {
                    Title = modelName,
                    Subtitle = localizer.GetLocalizedString(subtitleKey, defaultSubtitle),
                    Data = default(TModel),
                    IndexAction = options.ConfigureIndexAction(modelName, localizer),
                    EditAction = options.ConfigureEditAction(modelName, localizer),
                    DeleteAction = options.ConfigureDeleteAction(modelName, localizer),
                    KeyPropertyName = options.KeyPropertyName,
                    Localizer = localizer,
                    UsePopups = options.UsePopups,
                };
            result.Columns = result.GetModelColumns(localizer);
            return result;
        }

        public static CrudEdit<TModel, TKey> CreateEditModel<TModel, TKey>(this ControllerOptions<TModel, TKey> options, IStringLocalizer localizer)
        where TModel : class, new() => CreateEditModel(options, localizer, "GenericCrud.Edit.Subtitle", "Edit");
        private static CrudEdit<TModel, TKey> CreateEditModel<TModel, TKey>(
            this ControllerOptions<TModel, TKey> options,
            IStringLocalizer localizer,
            string subtitleKey,
            string defaultSubtitle)
        where TModel : class, new()
        {
            var modelName = options.GetModelName(localizer);
            var result = new CrudEdit<TModel, TKey>()
                {
                    Title = modelName,
                    Subtitle = localizer.GetLocalizedString(subtitleKey, defaultSubtitle),
                    Data = default(TModel),
                    IndexAction = options.ConfigureIndexAction(modelName, localizer),
                    SaveAction = options.ConfigureSaveAction(modelName, localizer),
                    KeyPropertyName = options.KeyPropertyName,
                    Localizer = localizer,
                    UsePopups = options.UsePopups
                };
            result.Columns = result.GetModelColumns(localizer);
            return result;
        }
        public static CrudEdit<TModel, TKey> CreateCreateModel<TModel, TKey>(this ControllerOptions<TModel, TKey> options, IStringLocalizer localizer)
        where TModel : class, new() => CreateEditModel(options, localizer, "GenericCrud.Create.Subtitle", "Create");
    }
}