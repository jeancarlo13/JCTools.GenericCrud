using System;
using JCTools.GenericCrud.Controllers;
using JCTools.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;

namespace Test.Controllers
{
    [Route("Movie")]
    public class MovieController : Generic<Data.Context, Models.Movie>
    {
        public MovieController(IViewRenderService renderService) : base(renderService, new Data.Context())
        { 
            
        }
    }
}