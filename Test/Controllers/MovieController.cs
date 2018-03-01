using System;
using JCTools.GenericCrud.Controllers;
using JCTools.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Test.Controllers
{
    [Route("Movie")]
    public class MovieController : GenericController<Data.Context, Models.Movie, int>
    {
        public MovieController(IServiceProvider serviceProvider) 
        : base(serviceProvider, new Data.Context())
        { 
            // Settings.ListOptions.Subtitle = "Lista";
        }
    }
}