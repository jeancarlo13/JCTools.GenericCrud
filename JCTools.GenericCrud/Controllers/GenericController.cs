using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JCTools.GenericCrud.DataAnnotations;
using JCTools.GenericCrud.Helpers;
using JCTools.GenericCrud.Models;
using JCTools.GenericCrud.Services;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace JCTools.GenericCrud.Controllers
{
    /// <summary>
    /// Used for create the controllers that are the entry points for the custom cruds
    /// </summary>
    [CrudConstraint]
    public class GenericController : Controller, IGenericController
    {
        /// <summary>
        /// The database context instance to be used in the database operations 
        /// </summary>
        protected readonly DbContext DbContext;

        /// <summary>
        /// Instance of the Settings of the crud.
        /// </summary>
        /// <remarks>You can use its for customize the current crud only</remarks>
        protected IViewModel Settings { get; set; }

        /// <summary>
        /// The instance of <see cref="IViewRenderService"/> used for render the embedded views
        /// </summary>
        private IViewRenderService _renderingService;

        /// <summary>
        /// The instance of <see cref="IStringLocalizer" /> used of the internationalization 
        /// and localization of the string
        /// </summary>
        private readonly ICrudLocalizer _localizer;

        /// <summary>
        /// The instance of <see cref="ILogger"/> used for send to log the message of the controller
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The instance of <see cref="ILoggerFactory"/> used for create new logs
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;
        /// <summary>
        /// The CRUD type to be used for configure the instance
        /// </summary>
        internal ICrudType CrudType;

        /// <summary>
        /// The name of the property used how to key/id of the model
        /// </summary>
        private readonly string _keyPropertyName;

        /// <summary>
        /// Initialize an instace of the controller with the required services
        /// </summary>
        /// <param name="serviceProvider">Instance of <see cref="IServiceProvider" /> used 
        /// of access to the configured services into the startup class</param>
        /// <param name="renderingService">The instance of <see cref="IViewRenderService"/> used 
        /// for render the embedded views</param>
        /// <param name="loggerFactory">The instance of <see cref="ILoggerFactory"/> used for create new logs</param>
        /// <param name="keyPropertyName">The name of the property used how to key/id of the model</param>
        public GenericController(
            IServiceProvider serviceProvider,
            IViewRenderService renderingService,
            ILoggerFactory loggerFactory,
            string keyPropertyName = "Id"
        )
        {
            if (string.IsNullOrWhiteSpace(keyPropertyName))
                throw new ArgumentException(
                    $"'{nameof(keyPropertyName)}' cannot be null or whitespace.",
                    nameof(keyPropertyName)
                );
            _keyPropertyName = keyPropertyName;

            DbContext = serviceProvider.GetRequiredService(Configurator.DatabaseContextType) as DbContext;
            if (DbContext == null)
                throw new ArgumentException("Failure generating the database context.");

            _localizer = serviceProvider.GetRequiredService<ICrudLocalizer>()
                ?? throw new InvalidOperationException($"Failure getting the {nameof(ICrudLocalizer)} service.");

            _renderingService = renderingService ?? throw new ArgumentNullException(nameof(renderingService));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger(this.GetType());
        }

        /// <summary>
        /// Called before the action method is invoked. 
        /// It's required for the <see cref="Settings"/> property initialization
        /// </summary>
        /// <param name="filterContext">The action executing context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            InitSettings(filterContext);
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Called before the action method is invoked. 
        /// It's required for the <see cref="Settings"/> property initialization
        /// </summary>
        /// <param name="context">The action executing context.</param>
        /// <param name="next">The <see cref="ActionExecutionDelegate"/> to execute. Invoke
        /// this delegate in the body of <see cref="Controller.OnActionExecutionAsync(ActionExecutingContext,ActionExecutionDelegate)"/>
        /// to continue execution of the action.</param>
        /// <returns>A <see cref="Task"/> instance.</returns>
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            InitSettings(context);
            return base.OnActionExecutionAsync(context, next);
        }

        /// <summary>
        /// Tries initializes the <see cref="Settings"/> property
        /// </summary>
        /// <param name="context">The action executing context.</param>
        protected void InitSettings(ActionExecutingContext context)
        {
            if (Settings == null)
            {
                if (!context.ActionArguments.TryGetValue(Constants.EntitySettingsRouteKey, out object entityName)
                    && !context.RouteData.Values.TryGetValue(Constants.EntitySettingsRouteKey, out entityName))
                {
                    throw new InvalidOperationException($"Entity name not found.");
                }

                CrudType = entityName is ICrudType
                    ? entityName as ICrudType
                    : Configurator.Options.Models[entityName.ToString()];
                if (CrudType == null)
                    throw new InvalidOperationException($"Configured model not found for {entityName}");

                var type = typeof(CrudModel<,>).MakeGenericType(CrudType.ModelType, CrudType.KeyPropertyType);
                var constructor = type.GetConstructor(
                    bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
                    binder: null,
                    types: new Type[]
                    {
                        typeof(ICrudType),
                        typeof(IStringLocalizer),
                        typeof(IUrlHelper),
                        typeof(ILoggerFactory)
                    },
                    modifiers: null
                );

                Settings = constructor?.Invoke(new object[] { CrudType, _localizer, Url, _loggerFactory }) as IViewModel
                    ?? null;
            }
        }
        /// <summary>
        /// Allows render the index view
        /// </summary>
        /// <param name="entitySettings">The settings of the desired CRUD</param>
        /// <param name="id">The last id affect for the crud</param>
        /// <param name="message">The identifier of the message to show at the user</param>
        [Route("{entitySettings}/{id?}/{message?}")]
        public virtual async Task<IActionResult> Index(
            ICrudType entitySettings,
            string id,
            IndexMessages message = IndexMessages.None
        )
        {
            var model = Settings as IIndexModel;
            model.SetData(DbContext);
            model.Message = new ViewMessage()
            {
                Text = message != IndexMessages.None
                    ? _localizer[$"GenericCrud.Index.{message.ToString()}Message"]
                    : string.Empty,
                CssClass = message == IndexMessages.EditSuccess ? "alert-success"
                    : message == IndexMessages.CreateSuccess ? "alert-success"
                    : message == IndexMessages.DeleteSuccess ? "alert-success"
                    : "alert-info"
            };

            if (!string.IsNullOrWhiteSpace(id))
                model.SetId(id);

            model.CurrentProcess = CrudProcesses.Index;
            return Content(
                await _renderingService.RenderToStringAsync(nameof(Index), model, ViewData),
                "text/html"
            );
        }

        /// <summary>
        /// Allows get a javascript embedded file
        /// </summary>
        /// <param name="entitySettings">The settings of the desired CRUD</param>
        /// <param name="fileName">The name of the desired file</param>
        /// <returns>A file with the found file content</returns>        
        [Route("{entitySettings}/{fileName}.js")]
        public IActionResult GetScript(ICrudType entitySettings, string fileName)
        {
            var assembly = typeof(GenericController).GetTypeInfo().Assembly;

            // This shows the available items.
            string[] resources = assembly.GetManifestResourceNames();

            var namespase = typeof(Configurator).Namespace;
            var stream = assembly.GetManifestResourceStream($"{namespase}.js.{fileName}.js");

            if (stream == null)
                return NotFound();

            using (var reader = new StreamReader(stream))
            {
                var bytes = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                return File(bytes, "application/javascript");
            }
        }

        /// <summary>
        /// Allows render the details view
        /// </summary>
        /// <param name="entitySettings">The settings of the desired CRUD</param>
        /// <param name="id">The id of the entity to show into the view</param>
        [HttpGet]
        [Route("{entitySettings}/{id}/[action]")]
        public virtual Task<IActionResult> Details(ICrudType entitySettings, string id)
            => ShowDetailsAsync(id);

        /// <summary>
        /// Allows render the delete view
        /// </summary>
        /// <param name="entitySettings">The settings of the desired CRUD</param>
        /// <param name="id">The id of the entity to show into the view</param>
        [HttpGet]
        [Route("{entitySettings}/{id}/[action]")]
        public virtual Task<IActionResult> Delete(ICrudType entitySettings, string id)
            => ShowDetailsAsync(id, isForDeletion: true);

        /// <summary>
        /// Allows render the delete view
        /// </summary>
        /// <param name="entitySettings">The settings of the desired CRUD</param>
        /// <param name="id">The id of the entity to show into the view</param>
        [HttpGet]
        [Route("{entitySettings}/{id}/[action]")]
        public virtual async Task<IActionResult> DeleteConfirm(ICrudType entitySettings, string id)
        {
            await Settings.SetDataAsync(DbContext, id);
            var entity = Settings.GetData().GetEntity();

            if (entity == null)
                return NotFound();

            DbContext.Remove(entity);

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "Failure deleting entity.");
                var message = _localizer["GenericCrud.UnableDeleteMessage"];
                ModelState.AddModelError("",
                    string.IsNullOrWhiteSpace(message) ?
                    "Unable to delete the data. Try again, and if the problem persists, see your system administrator." :
                    message
                );
            }

            return SendSuccessResponse(id, IndexMessages.DeleteSuccess);
        }

        /// <summary>
        /// Allows render the create view
        /// </summary>
        /// <param name="entitySettings">The settings of the desired CRUD</param>
        [HttpGet]
        [Route("{entitySettings}/[action]")]
        public virtual async Task<IActionResult> Create(ICrudType entitySettings)
        {
            var model = Settings as IEditModel;
            model.CurrentProcess = CrudProcesses.Create;
            return await RenderView(nameof(Edit), model, model.SaveAction);
        }

        /// <summary>
        /// Allows render the Edit view
        /// </summary>
        /// <param name="entitySettings">The settings of the desired CRUD</param>
        /// <param name="id">The id of the entity to edit into the view</param>
        [HttpGet]
        [Route("{entitySettings}/{id}/[action]")]
        public virtual async Task<IActionResult> Edit(ICrudType entitySettings, string id)
        {
            await Settings.SetDataAsync(DbContext, id);
            var entity = Settings.GetData();

            return await Edit(id, entity);
        }

        /// <summary>
        /// Allows save new entities
        /// </summary>
        /// <param name="entitySettings">The settings of the desired CRUD</param>
        /// <param name="entityModel">The entity to save</param>
        [HttpPost]
        [Route("{entitySettings}/[action]")]
        public virtual async Task<IActionResult> Save(
            ICrudType entitySettings,
            [FromForm] object entityModel
        )
        {
            ModelState.Remove(Settings.KeyPropertyName);
            if (ModelState.IsValid)
            {
                await DbContext.AddAsync(entityModel);

                try
                {
                    await DbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogWarning(ex, "Failure saving changes.");
                    AddSaveChangesErrorMessage();
                    return await Create(entitySettings);
                }

                return SendSuccessResponse(
                    Settings.GetKeyPropertyValue(entityModel).ToString(),
                    IndexMessages.CreateSuccess
                );
            }

            return await Create(entitySettings);
        }

        /// <summary>
        /// Allows valid and save the changes into the specified entity
        /// </summary>
        /// <param name="entitySettings">The settings of the desired CRUD</param>
        /// <param name="entityModel">Instance with the changes of the entity to save</param>
        /// <param name="id">The id of the entity to change</param>
        [HttpPost]
        [ActionName(GenericCrud.Settings.Route.SaveChangesActionName)]
        [Route("{entitySettings}/{id}/[action]")]
        public virtual async Task<IActionResult> SaveChangesAsync(
            ICrudType entitySettings,
            string id,
            [FromForm] object entityModel
        )
        {
            Settings.SetId(id);
            var key = Settings.GetId();

            var model = Settings as IEditModel;
            model.SetData(entityModel);

            var modelId = model.GetId();
            if (!entitySettings.KeyPropertyIsEditable && !modelId.Equals(key))
                return NotFound();

            if (ModelState.IsValid)
            {
                await Settings.SetDataAsync(DbContext, id);
                var entity = Settings.GetData().GetEntity();

                if (entity == null)
                    return NotFound();

                if (!entitySettings.KeyPropertyIsEditable || modelId.Equals(key))
                    DbContext.Entry(entity).CurrentValues.SetValues(entityModel);
                else
                {
                    DbContext.Remove(entity);
                    await DbContext.AddAsync(entityModel);
                }

                try
                {
                    await DbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogWarning(ex, "Failure saving changes.");
                    AddSaveChangesErrorMessage();
                }

                return SendSuccessResponse(modelId.ToString(), IndexMessages.EditSuccess);
            }

            return await Edit(id, Settings.GetData());
        }

        /// <summary>
        /// Allows render the details view
        /// </summary>
        /// <param name="id">The id of the entity to show into the view</param>
        /// <param name="isForDeletion">True if the view will used for delete the related entity;
        /// another, false</param>
        /// <returns>The task to be invoked</returns>
        private async Task<IActionResult> ShowDetailsAsync(string id, bool isForDeletion = false)
        {
            var model = Settings as IDetailsModel;
            await model.SetDataAsync(DbContext, id);

            var entity = model.GetData().GetEntity();
            if (entity == null)
                return NotFound();

            model.CurrentProcess = isForDeletion ? CrudProcesses.Delete : CrudProcesses.Details;
            return await RenderView(nameof(Details), model, isForDeletion ? model.DeleteAction : null);
        }

        /// <summary>
        /// Allows render the Edit view
        /// </summary>
        /// <param name="id">The id of the entity to edit into the view</param>
        /// <param name="entity">The entity to edit</param>
        private async Task<IActionResult> Edit(string id, IEntityData entity)
        {
            if (entity == null)
                return NotFound();

            var model = Settings as IEditModel;
            model.SetData(entity.GetEntity());

            model.CurrentProcess = CrudProcesses.Edit;
            return await RenderView(nameof(Edit), model, model.SaveAction);
        }
        /// <summary>
        /// Add a the current Model State a message with the save changes error
        /// </summary>
        private void AddSaveChangesErrorMessage()
        {
            var message = _localizer["GenericCrud.UnableSaveChangesMessage"];
            ModelState.AddModelError("",
                string.IsNullOrWhiteSpace(message) ?
                "Unable to save changes. Try again, and if the problem persists, see your system administrator." :
                message
            );
        }

        /// <summary>
        /// Generate the correctly response using the specified settings and arguments
        /// </summary>
        /// <param name="id">The id of the affected entity</param>
        /// <param name="message">The message to be sent into the response</param>
        /// <returns>The generated action result</returns>
        private IActionResult SendSuccessResponse(string id, IndexMessages message = IndexMessages.None)
        {
            var redirectUrl = Url.RouteUrl(
                GenericCrud.Settings.Route.RedirectIndexActionNamePattern,
                CrudType as ICrudTypeRoutable,
                id,
                message
            );

            if (Settings.UseModals)
                return Json(new JsonResponse { Success = true, RedirectUrl = redirectUrl });
            // else if (string.IsNullOrWhiteSpace(id))
            //     return RedirectToAction(nameof(Index), new { message = message });
            else
                return Redirect(redirectUrl);
        }

        /// <summary>
        /// Generate the view to be display to the user
        /// </summary>
        /// <param name="view">The name of the view to render</param>
        /// <param name="model">The model with the data to display</param>
        /// <param name="commitAction">The action to be invoked when the user desired save the changes made</param>
        /// <returns>The task to be invoked</returns>
        private async Task<IActionResult> RenderView(
            string view,
            IViewModel model,
            CrudAction commitAction = null)
        {
            Task<string> contentTask;
            if (Settings.UseModals)
            {
                var popupModel = new Popup()
                {
                    Model = model,
                    InnerView = view,
                    CommitAction = commitAction
                };

                contentTask = _renderingService.RenderToStringAsync("_popup", popupModel, ViewData);
            }
            else
                contentTask = _renderingService.RenderToStringAsync(view, model, ViewData);

            return Content(await contentTask, "text/html");
        }
    }
}