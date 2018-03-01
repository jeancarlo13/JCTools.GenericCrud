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
    public class GenericController<TDbContext, TModel, TKey> : Controller
    where TDbContext : DbContext
    where TModel : class, new()
    {
        protected readonly TDbContext DbContext;
        protected ControllerOptions<TModel> Settings
        {
            get;
            set;
        }
        private IViewRenderService _renderingService;
        private readonly IStringLocalizer _localizer;
        private readonly ILogger _logger;
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

        private void InitSettings(string keyPropertyName)
        {
            Settings = new ControllerOptions<TModel>(Configurator.Options, keyPropertyName);
            Settings.ListOptions = Settings.CreateListModel(_localizer);
            Settings.DetailsOptions = Settings.CreateDetailsModel(_localizer);
            Settings.EditOptions = Settings.CreateEditModel(_localizer);
        }

        public async Task<IActionResult> Index()
        {
            var all = DbContext.Set<TModel>();
            var model = Settings.ListOptions;
            model.Data = all;

            return Content(
                await _renderingService.RenderToStringAsync(
                    nameof(Index),
                    model
                ),
                "text/html"
            );
        }

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

        [Route("{id}/edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(TKey id)
        {

            var entity = await DbContext.Set<TModel>().FindAsync(id);

            if (entity == null)
                return NotFound();

            var model = Settings.EditOptions;
            model.Data = entity;

            return Content(
                await _renderingService.RenderToStringAsync(
                    nameof(Edit),
                    model
                ),
                "text/html"
            );
        }

        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> SaveChangesAsync(TModel model)
        {
            var id = Convert.ChangeType(model.GetType().GetProperty(Settings.KeyPropertyName)?.GetValue(model), typeof(TKey));
            if (id == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                var saved = await DbContext.Set<TModel>().FindAsync(id);

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
                return RedirectToAction(nameof(Index));
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