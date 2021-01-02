using System;
using System.Linq;
using JCTools.GenericCrud.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Test3._1.Controllers
{
    public class MovieController : GenericController<Data.Context, Models.Movie, int>
    {
        public MovieController(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Settings.UseModals = false;
            Settings.Subtitle = "All entities";
            ViewBag.Countries = DbContext.Countries.ToList();
            base.OnActionExecuting(filterContext);
        }


    }
}