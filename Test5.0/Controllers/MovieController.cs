using System;
using System.Linq;
using JCTools.GenericCrud.Controllers;
using JCTools.GenericCrud.DataAnnotations;
using JCTools.GenericCrud.Models;
using JCTools.GenericCrud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Test5._0.Data;
using Test5._0.Models;

namespace Test5._0.Controllers
{
    [CrudConstraint(typeof(Movie))]
    [Authorize]
    public class MovieController : GenericController
    {
        public MovieController(
            IServiceProvider serviceProvider,
            IViewRenderService renderingService,
            IStringLocalizerFactory localizerFactory,
            ILoggerFactory loggerFactory
        )
            : base(serviceProvider, renderingService, loggerFactory, nameof(Models.Movie.Id))
        {
            // add your custom logic here
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Call the initialization of the Settings property
            base.InitSettings(filterContext);

            // Add your custom settings here, eg;
            Settings.UseModals = false; // disabled the modals
            Settings.Subtitle = "All entities"; // change the default subtitle

            // Customizing the Icons and Buttons Classes of the Index Page
            var index = Settings as IIndexModel;
            index.NewAction.IconClass = "fa fa-plus-circle";
            index.NewAction.ButtonClass = "btn btn-success btn-sm";

            index.DetailsAction.IconClass = "fa fa-info";
            index.DetailsAction.ButtonClass = "btn btn-info btn-sm";

            index.EditAction.IconClass = "fa fa-edit";
            index.EditAction.ButtonClass = "btn btn-warning btn-sm";

            index.DeleteAction.IconClass = "fa fa-eraser";

            // other things
            ViewBag.Countries = (DbContext as Context).Countries.ToList();

            base.OnActionExecuting(filterContext);
        }
    }
}