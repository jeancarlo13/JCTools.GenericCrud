using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JCTools.GenericCrud.Helpers;
using JCTools.GenericCrud.Models;
using JCTools.GenericCrud.Services;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace JCTools.GenericCrud.Controllers
{
    /// <summary>
    /// Used for create the controllers that are the entry points for the custom cruds
    /// </summary>
    /// <typeparam name="TContext">The type of the database context to be used by get/stored the entities </typeparam>
    /// <typeparam name="TModel">The type of the model that represents the entities to modified</typeparam>
    /// <typeparam name="TKey">The type of the property identifier of the entity model</typeparam>
    public class GenericController<TContext, TModel, TKey> : Controller
        where TContext : DbContext
        where TModel : class, new()
    {
        /// <summary>
        /// The database context instance to be used in the database operations 
        /// </summary>
        protected readonly TContext DbContext;
        /// <summary>
        /// Instance of the Settings of the crud.
        /// </summary>
        /// <remarks>You can use its for customize the current crud only</remarks>
        protected ControllerOptions<TModel, TKey> Settings { get; set; }
        /// <summary>
        /// The instance of <see cref="IViewRenderService"/> used for render the embedded views
        /// </summary>
        private IViewRenderService _renderingService;
        /// <summary>
        /// The instance of <see cref="IStringLocalizer" /> used of the internationalization and localization of the string
        /// </summary>
        private readonly IStringLocalizer _localizer;
        /// <summary>
        /// The instance of <see cref="ILogger"/> used for send to log the message of the controller
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// Create an instace of the controller with the specific parameter
        /// </summary>
        /// <param name="serviceProvider">Instance of <see cref="IServiceProvider" /> used of access to the configured services into the startup class</param>
        /// <param name="keyPropertyName">The name of the model property to be used how to identifier of the entities</param>
        public GenericController(
            IServiceProvider serviceProvider,
            string keyPropertyName = "Id"
        )
        {
            DbContext = serviceProvider.GetRequiredService<TContext>();

            if (DbContext == null)
                throw new ArgumentException("Failure generating the database context.");

            _renderingService = serviceProvider.GetService(typeof(IViewRenderService)) as IViewRenderService;
            _localizer = serviceProvider.GetService(typeof(IStringLocalizer)) as IStringLocalizer;
            _logger = (serviceProvider.GetService(typeof(ILoggerFactory)) as ILoggerFactory)
                .CreateLogger<GenericController<TContext, TModel, TKey>>();

            Settings = new ControllerOptions<TModel, TKey>(Configurator.Options, keyPropertyName, _localizer);
        }

        /// <summary>
        /// Allows render the index view
        /// </summary>
        /// <param name="id">The last id affect for the crud</param>
        /// <param name="message">The identifier of the message to show at the user</param>
        public virtual async Task<IActionResult> Index(object id = null, IndexMessages message = IndexMessages.None)
        {
            var all = DbContext.Set<TModel>();
            var model = Settings.ListOptions;
            model.SetData(all);
            model.Message = message != IndexMessages.None ? _localizer[$"GenericCrud.Index.{message.ToString()}Message"] : string.Empty;
            model.MessageClass =
                message == IndexMessages.EditSuccess ? "alert-success" :
                message == IndexMessages.CreateSuccess ? "alert-success" :
                message == IndexMessages.DeleteSuccess ? "alert-success" :
                "alert-info";
            if (id != null)
            {
                try
                {
                    var realId = (TKey)Convert.ChangeType(id, typeof(TKey));
                    model.SetId(realId);
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, $"The '{id}' is not a valid value for {typeof(TKey)}.");
                }
            }

            return Content(
                await _renderingService.RenderToStringAsync(
                    nameof(Index),
                    model,
                    ViewData
                ),
                "text/html"
            );
        }

        /// <summary>
        /// Allows render the details view
        /// </summary>
        /// <param name="id">The id of the entity to show into the view</param>
        [HttpGet]
        public virtual async Task<IActionResult> Details(TKey id)
        {
            var entity = await DbContext.Set<TModel>().FindAsync(id);

            if (entity == null)
                return NotFound();

            var model = Settings.DetailsOptions;
            model.SetData(entity);

            return await RenderView(nameof(Details), model);
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
            IBase model,
            CrudAction commitAction = null)
        {
            object popupModel = model;
            if (Settings.UseModals)
            {
                popupModel = new Popup()
                {
                    Model = model,
                    InnerView = view,
                    CommitAction = commitAction
                };

                view = "_popup";
            }

            return Content(
                await _renderingService.RenderToStringAsync(view, popupModel, ViewData),
                "text/html"
            );
        }

        /// <summary>
        /// Allows render the delete view
        /// </summary>
        /// <param name="id">The id of the entity to show into the view</param>
        [HttpGet]
        public virtual async Task<IActionResult> Delete(TKey id)
        {
            var entity = await DbContext.Set<TModel>().FindAsync(id);

            if (entity == null)
                return NotFound();

            var model = Settings.DeleteOptions;
            model.SetData(entity);
            model.SetId(id);

            ViewBag.IsDelete = true;

            var action = Settings.ConfigureDeleteAction(Settings.GetModelName(_localizer), _localizer);
            action.Url = Url.Action(nameof(DeleteConfirm), new
            {
                id
            });

            return await RenderView(nameof(Details), model, action);
        }

        /// <summary>
        /// Allows render the delete view
        /// </summary>
        /// <param name="id">The id of the entity to show into the view</param>
        [HttpGet]
        public virtual async Task<IActionResult> DeleteConfirm(TKey id)
        {
            var entity = await DbContext.Set<TModel>().FindAsync(id);

            if (entity == null)
                return NotFound();

            var saved = await DbContext.Set<TModel>().FindAsync(id);

            if (saved == null)
                return NotFound();

            DbContext.Remove(saved);

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

            return SendSuccessResponse(nameof(Index), null, IndexMessages.DeleteSuccess);
        }

        /// <summary>
        /// Allows render the create view
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> Create()
        {
            var model = Settings.CreateOptions;

            ViewBag.Action = Url.Action(nameof(Save));

            var action = Settings.ConfigureSaveAction(Settings.GetModelName(_localizer), _localizer);
            action.Url = ViewBag.Action;

            return await RenderView(nameof(Edit), model, action);
        }

        /// <summary>
        /// Allows save new entities
        /// </summary>
        /// <param name="model">The entity to save</param>
        [HttpPost]
        public virtual async Task<IActionResult> Save([FromForm] TModel model)
        {
            ModelState.Remove(Settings.CreateOptions.KeyPropertyName);
            if (ModelState.IsValid)
            {

                await DbContext.AddAsync(model);

                try
                {
                    await DbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogWarning(ex, "Failure saving changes.");
                    AddSaveChangesErrorMessage();
                }

                return SendSuccessResponse(
                    nameof(Index),
                    typeof(TModel).GetProperty(Settings.CreateOptions.KeyPropertyName)?.GetValue(model),
                    IndexMessages.CreateSuccess
                );
            }

            return await Create();
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
        /// Allows render the Edit view
        /// </summary>
        /// <param name="id">The id of the entity to edit into the view</param>
        [HttpGet]
        public virtual async Task<IActionResult> Edit(TKey id)
        {
            var entity = await DbContext.Set<TModel>().FindAsync(id);

            if (entity == null)
                return NotFound();

            return await Edit(id, entity);
        }

        /// <summary>
        /// Allows render the Edit view
        /// </summary>
        /// <param name="id">The id of the entity to edit into the view</param>
        /// <param name="entity">The entity to edit</param>
        private async Task<IActionResult> Edit(TKey id, TModel entity)
        {
            var model = Settings.EditOptions;
            model.SetData(entity);
            model.SetId(id);

            ViewBag.Action = Url.Action(nameof(SaveChangesAsync), new
            {
                id = id
            });

            var action = Settings.ConfigureSaveAction(Settings.GetModelName(_localizer), _localizer);
            action.Url = ViewBag.Action;

            return await RenderView(nameof(Edit), model, action);
        }

        /// <summary>
        /// Allows valid and save the changes into the specified entity
        /// </summary>
        /// <param name="model">Instance with the changes of the entity to save</param>
        /// <param name="id">The id of the entity to change</param>
        [HttpPost]
        public virtual async Task<IActionResult> SaveChangesAsync(TKey id, [FromForm] TModel model)
        {
            var key = (TKey)Convert.ChangeType(model.GetType().GetProperty(Settings.KeyPropertyName)?.GetValue(model), typeof(TKey));
            if (!id.Equals(key))
                return NotFound();

            if (ModelState.IsValid)
            {
                var saved = await DbContext.Set<TModel>().FindAsync(key);

                if (saved == null)
                    return NotFound();

                DbContext.Entry(saved).CurrentValues.SetValues(model);

                try
                {
                    await DbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogWarning(ex, "Failure saving changes.");
                    AddSaveChangesErrorMessage();
                }

                return SendSuccessResponse(nameof(Index), id, IndexMessages.EditSuccess);
            }

            return await Edit(id, model);
        }

        /// <summary>
        /// Generate the correctly response using the specified settings and arguments
        /// </summary>
        /// <param name="action">The action to be redirect if the modals was not used</param>
        /// <param name="id">The id of the affected entity</param>
        /// <param name="message">The message to be sent into the response</param>
        /// <returns>The generated action result</returns>
        private IActionResult SendSuccessResponse(string action, object id, IndexMessages message = IndexMessages.None)
        {
            if (Settings.UseModals)
            {
                var strId = id == null ? string.Empty : $"{id}/index";
                var strMessage = message != IndexMessages.None ? $"?message={message}" : string.Empty;
                return Json(new JsonResponse
                {
                    Success = true,
                    RedirectUrl = Url.Action(nameof(Index), new
                    {
                        message = message,
                        id = id
                    })
                });
            }
            else
                return RedirectToAction(action, new
                {
                    message = id,
                    id = message
                });
        }

        /// <summary>
        /// Allows get a javascript embedded file
        /// </summary>
        /// <param name="fileName">The name of the desired file</param>
        /// <returns>A file with the found file content</returns>
        public IActionResult GetScript(string fileName)
        {
            var assembly = Settings.GetType().GetTypeInfo().Assembly;

            // This shows the available items.
            string[] resources = assembly.GetManifestResourceNames();

            var stream = assembly.GetManifestResourceStream($"JCTools.GenericCrud.js.{fileName}.js");

            if (stream == null)
                return NotFound();

            using (var reader = new StreamReader(stream))
            {
                var bytes = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                return File(bytes, "application/javascript");
            }
        }
    }
}