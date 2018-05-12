using System;
using System.Linq;
using System.Threading.Tasks;
using JCTools.GenericCrud.Controllers;
using JCTools.GenericCrud.Models;
using JCTools.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Test.Controllers
{
    public class MovieController : GenericController<Data.Context, Models.Movie, int>
    {
        public MovieController(IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            // Settings.ListOptions.Subtitle = "Lista";
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.Countries = DbContext.Countries.ToList();
            base.OnActionExecuting(filterContext);
        }
        

    }
}