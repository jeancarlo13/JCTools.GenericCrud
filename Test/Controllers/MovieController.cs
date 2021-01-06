using System;
using System.Linq;
using JCTools.GenericCrud.Controllers;
using JCTools.GenericCrud.DataAnnotations;
using JCTools.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    [CrudConstraint(typeof(Movie))]
    public class MovieController : GenericController
    {
        public MovieController(
            IServiceProvider serviceProvider,
            IViewRenderService renderingService,
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
            Settings.UseModals = false;
            Settings.Subtitle = "All entities";
            ViewBag.Countries = (DbContext as Context).Countries.ToList();

            base.OnActionExecuting(filterContext);
        }
    }
}