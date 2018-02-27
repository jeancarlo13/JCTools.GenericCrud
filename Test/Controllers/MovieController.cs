using System;
using JCTools.GenericCrud.Controllers;
using JCTools.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Test.Controllers
{
    [Route("Movie")]
    public class MovieController : GenericController<Data.Context, Models.Movie, int>
    {
        public MovieController(IViewRenderService renderService, IStringLocalizer<HomeController> localizer) 
        : base(renderService, new Data.Context(), localizer, nameof(Models.Movie.Id))
        { 
            // Settings.ListOptions.Subtitle = "Lista";
        }
    }
}