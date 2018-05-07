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
        /// <summary>
        /// Allows render the create view
        /// </summary>
        /// <param name="id">The id of the entity to edit into the view</param>
        [HttpGet]
        public override async Task<IActionResult> Create()
        {
            return await base.Create();
        }
    }
}