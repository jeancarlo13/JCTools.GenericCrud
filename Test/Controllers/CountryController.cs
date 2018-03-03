using System;
using JCTools.GenericCrud.Controllers;
using JCTools.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Test.Controllers
{
    [Route("Country")]
    public class CountryController : GenericController<Data.Context, Models.Country, int>
    {
        public CountryController(IServiceProvider serviceProvider) 
        : base(serviceProvider, new Data.Context())
        { 
            // Settings.ListOptions.Subtitle = "Lista";
        }
    }
}