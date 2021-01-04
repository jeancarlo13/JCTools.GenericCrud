using System;
using System.Linq;
using JCTools.GenericCrud.Controllers;
using JCTools.GenericCrud.Services;
using JCTools.GenericCrud.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Test3._1.Data;
using Test3._1.Models;

namespace Test3._1.Controllers
{
    [CrudConstraint(typeof(Movie))]
    public class MovieController : GenericController
    {
        public MovieController(
            IServiceProvider serviceProvider,
            IViewRenderService renderingService,
            IStringLocalizerFactory localizerFactory,
            ILoggerFactory loggerFactory
        )
            : base(serviceProvider, renderingService, localizerFactory, loggerFactory, "Id")
        { }

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