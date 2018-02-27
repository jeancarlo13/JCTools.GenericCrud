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

namespace JCTools.GenericCrud.Controllers
{
    public class GenericController<TDbContext, TModel, TKey> : Controller
    where TDbContext : DbContext
    where TModel : class, new()
    {
        protected readonly TDbContext DbContext;
        protected readonly ControllerOptions<TModel> Settings;
        private IViewRenderService _renderingService;
        private readonly IStringLocalizer _localizer;

        public GenericController(
            IViewRenderService renderingService,
            TDbContext context,
            IStringLocalizer localizer,
            string keyPropertyName
        )
        {
            DbContext = context;
            _renderingService = renderingService;
            _localizer = localizer;

            Settings = new ControllerOptions<TModel>(Configurator.Options, keyPropertyName);
            Settings.ListOptions = Settings.CreateListModel(_localizer);
            Settings.DetailsOptions = Settings.CreateDetailsModel(_localizer);
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
    }
}