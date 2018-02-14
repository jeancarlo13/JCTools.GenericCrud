using System;
using JCTools.GenericCrud.Helpers;
using JCTools.GenericCrud.Models;
using JCTools.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace JCTools.GenericCrud.Controllers
{
    public class Generic<TDbContext, TModel> : Controller
    where TDbContext : DbContext
    where TModel : class, new()
    {
        protected readonly TDbContext DbContext;
        protected readonly Options Options;
        private IViewRenderService _renderService;
        public Generic(IViewRenderService renderService, TDbContext context)
        {
            DbContext = context;
            _renderService = renderService;
            Options = Configurator.Options;
        }
        public async Task<IActionResult> Index()
        {
            var all = DbContext.Set<TModel>();
            var model = all.CreateListModel(Options);

            return Content(
                await _renderService.RenderToStringAsync(
                    nameof(Index), //$"~/Views/Generic/{nameof(Index)}.cshtml",
                    model
                ),
                "text/html"
            );
        }

    }
}