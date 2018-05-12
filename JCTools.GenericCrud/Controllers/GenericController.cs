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
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace JCTools.GenericCrud.Controllers
{
    /// <summary>
    /// Used for create the controllers that are theentry points for the custom cruds
    /// </summary>
    /// <typeparam name="TDbContext">the type of the database context to use</typeparam>
    /// <typeparam name="TModel">The type of the model to use related with the <see cref="TDbContext"/> parameter </typeparam>
    /// <typeparam name="TKey">the type of the property key of the <see cref="TModel" /> parameter</typeparam>
    /// <returns></returns>
    public class GenericController<TDbContext, TModel, TKey> : Controller
    where TDbContext : DbContext
    where TModel : class, new()
    {
        /// <summary>
        /// The instance of the <see cref="TDbContext" />. You use in the database operations 
        /// </summary>
        protected readonly TDbContext DbContext;
        /// <summary>
        /// Instance of the Settings of the crud. You use for personalize the current crud
        /// </summary>
        protected ControllerOptions<TModel, TKey> Settings
        {
            get;
            set;
        }
        /// <summary>
        /// The instance of <see cref="IViewRenderService"/> used for render the embebed views
        /// </summary>
        private IViewRenderService _renderingService;
        /// <summary>
        /// The instance of <see cref="IStringLocalizer" /> used of the internazionalization and localization of the string
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
        /// <param name="context">Instance of the database context to use for the database operations</param>
        public GenericController(
            IServiceProvider serviceProvider,
            string keyPropertyName = "Id"
        )
        {
            if (Configurator.Options.ContextCreator == null)
                throw new ArgumentNullException(nameof(Configurator.Options.ContextCreator));
            else
                DbContext = Configurator.Options.ContextCreator.Invoke() as TDbContext;

            _renderingService = serviceProvider.GetService(typeof(IViewRenderService)) as IViewRenderService;
            _localizer = serviceProvider.GetService(typeof(IStringLocalizer)) as IStringLocalizer;
            _logger = (serviceProvider.GetService(typeof(ILoggerFactory)) as ILoggerFactory)
                .CreateLogger<GenericController<TDbContext, TModel, TKey>>();

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
        /// <param name="id">The id of the entity to edit into the view</param>
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
        public virtual async Task<IActionResult> Save([FromForm]TModel model)
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
        /// Add a the current <see cref="Controller.ModelState"/> a message with the save changes error
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
        // <summary>
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
        public virtual async Task<IActionResult> SaveChangesAsync(TKey id, [FromForm]TModel model)
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

        private IActionResult SendSuccessResponse(string action, object id, IndexMessages message = IndexMessages.None)
        {
            if (Settings.UseModals)
            {
                var strId = id == null ? string.Empty : $"{id}/index";
                var strMessage = message != IndexMessages.None ? $"?message={message}" : string.Empty;
                return Json(new JsonResponse
                {
                    Success = true,
                    RedirectUrl = Url.Action(nameof(Index), new {                  
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

        public FileResult GetScript(string fileName)
        {
            var assembly = Settings.GetType().GetTypeInfo().Assembly;

            // This shows the available items.
            string[] resources = assembly.GetManifestResourceNames();

            var stream = assembly.GetManifestResourceStream($"JCTools.GenericCrud.js.{fileName}.js");

            using (var reader = new StreamReader(stream))
            {
                var bytes = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                return File(bytes, "application/javascript");
            }
        }
    }
}