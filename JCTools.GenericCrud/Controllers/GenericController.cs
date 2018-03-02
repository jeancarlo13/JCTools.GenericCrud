using System;
using System.Threading.Tasks;
using JCTools.GenericCrud.Helpers;
using JCTools.GenericCrud.Models;
using JCTools.GenericCrud.Services;
using JCTools.GenericCrud.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
    where TKey : struct
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
            TDbContext context
        )
        {
            DbContext = context;
            _renderingService = serviceProvider.GetService(typeof(IViewRenderService)) as IViewRenderService;
            _localizer = serviceProvider.GetService(typeof(IStringLocalizer)) as IStringLocalizer;
            _logger = (serviceProvider.GetService(typeof(ILoggerFactory)) as ILoggerFactory)
                .CreateLogger<GenericController<TDbContext, TModel, TKey>>();

            InitSettings("Id");
        }
        /// <summary>
        /// Create an instace of the controller with the specific parameter
        /// </summary>
        /// <param name="renderingService">The instance of the <see cref="IViewRenderService"/> used for render the embebed views</param>
        /// <param name="context">Instance of the database context to use for the database operations</param>
        /// <param name="localizer">The instance of <see cref="IStringLocalizer" /> used of the internazionalization and localization of the string</param>
        /// <param name="keyPropertyName">The name of the key property of the <see cref="TModel"/></param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> used for create the <see cref="ILogger"/> instnace</param>
        public GenericController(
            IViewRenderService renderingService,
            TDbContext context,
            IStringLocalizer localizer,
            string keyPropertyName,
            ILoggerFactory loggerFactory
        )
        {
            DbContext = context;
            _renderingService = renderingService;
            _localizer = localizer;
            _logger = loggerFactory.CreateLogger<GenericController<TDbContext, TModel, TKey>>();

            InitSettings(keyPropertyName);
        }
        /// <summary>
        /// Initialize the <see cref="Settings" /> property with the configured global <see cref="Options"/> 
        /// </summary>
        /// <param name="keyPropertyName">The name of the key property of the <see cref="TModel"/></param>
        private void InitSettings(string keyPropertyName)
        {
            Settings = new ControllerOptions<TModel, TKey>(Configurator.Options, keyPropertyName);
            Settings.ListOptions = Settings.CreateListModel(_localizer);
            Settings.DetailsOptions = Settings.CreateDetailsModel(_localizer);
            Settings.EditOptions = Settings.CreateEditModel<TModel, TKey>(_localizer);
        }
        /// <summary>
        /// Allows render the index view
        /// </summary>
        /// <param name="id">The last id affect for the crud</param>
        /// <param name="message">The identifier of the message to show at the user</param>
        [Route("")]
        [Route("Index")]
        [Route("Index/{message}/for/{id}")]
        public virtual async Task<IActionResult> Index(IndexMessages message, TKey? id)
        {
            var all = DbContext.Set<TModel>();
            var model = Settings.ListOptions;
            model.Data = all;
            model.Message = _localizer[$"GenericCrud.Index.{message.ToString()}Message"];
            model.MessageClass = message == IndexMessages.EditSuccess ? "alert-success" : "alert-info";
            if (id.HasValue)
                model.Id = id.Value;

            return Content(
                await _renderingService.RenderToStringAsync(
                    nameof(Index),
                    model
                ),
                "text/html"
            );
        }
        /// <summary>
        /// Allows render the details view
        /// </summary>
        /// <param name="id">The id of the entity to show into the view</param>
        [Route("{id}/details")]
        public async Task<IActionResult> Details(TKey id)
        {
            var entity = await DbContext.Set<TModel>().FindAsync(id);

            if (entity == null)
                return NotFound();

            var model = Settings.DetailsOptions;
            model.Data = entity;

            return Content(
                await _renderingService.RenderToStringAsync(
                    nameof(Details),
                    model
                ),
                "text/html"
            );
        }
        /// <summary>
        /// Allows render the Edit view
        /// </summary>
        /// <param name="id">The id of the entity to edit into the view</param>
        [Route("{id}/edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(TKey id)
        {
            var entity = await DbContext.Set<TModel>().FindAsync(id);

            if (entity == null)
                return NotFound();

            var model = Settings.EditOptions;
            model.Data = entity;
            model.Id = id;

            return Content(
                await _renderingService.RenderToStringAsync(
                    nameof(Edit),
                    model
                ),
                "text/html"
            );
        }
        /// <summary>
        /// Allows valid and save the changes into the specified entity
        /// </summary>
        /// <param name="model">Instance with the changes of the entity to save</param>
        /// <param name="id">The id of the entity to change</param>
        [Route("{id}/edit")]
        [HttpPost]
        public async Task<IActionResult> SaveChangesAsync(TKey id, TModel model)
        {
            var key = (TKey) Convert.ChangeType(model.GetType().GetProperty(Settings.KeyPropertyName)?.GetValue(model), typeof(TKey));
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
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index), new
                {
                    message = IndexMessages.EditSuccess,
                        id = id
                });
            }

            var realModel = Settings.EditOptions;
            realModel.Data = model;

            return Content(
                await _renderingService.RenderToStringAsync(
                    nameof(Edit),
                    realModel,
                    ViewData
                ),
                "text/html"
            );

        }
    }
}