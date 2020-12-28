using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Test3._1.Models;

namespace Test3._1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public HomeController(ILogger<HomeController> logger,
            IStringLocalizer<HomeController> localizer,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider
        )
        {
            _localizer = localizer;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("routes")]
        public IActionResult GetRoutes()
        {
            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items
            .Cast<ControllerActionDescriptor>()
            .Select(x => new
            {
                Action = x.ActionName,
                Controller = x.ControllerName,
                Name = x.AttributeRouteInfo?.Name,
                DisplayName = x.DisplayName,
                Template = x.AttributeRouteInfo?.Template,
                Method = x.MethodInfo.ToString(),
                RouteValues = x.RouteValues,
                Properties = x.Properties,
                ActionConstraints = x.ActionConstraints,
                Parameters = x.Parameters.Select(p => p.Name),
                BoundProperties = x.BoundProperties.Select(p => p.Name),
                FilterDescriptors = x.FilterDescriptors
            })
            .ToList();
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(routes), "application/json");
        }
    }
}
