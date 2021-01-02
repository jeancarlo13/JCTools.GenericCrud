using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
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
        private readonly EndpointDataSource _endpointsDataSource;

        public HomeController(ILogger<HomeController> logger,
            IStringLocalizer<HomeController> localizer,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            EndpointDataSource endpointsDataSource
        )
        {
            _localizer = localizer;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _endpointsDataSource = endpointsDataSource;
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

        [HttpGet("routes/{controllerName?}")]
        public IActionResult GetRoutes(string controllerName)
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
            .Where(x => string.IsNullOrWhiteSpace(controllerName) || x.Controller.Equals(controllerName))
            .ToList();
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(routes), "application/json");
        }

        [HttpGet("endpoints")]
        public IActionResult GetEndpoints()
        {
            var endpoints = _endpointsDataSource.Endpoints
                .Cast<RouteEndpoint>()
                .Select(x => new
                {
                    x.Order,
                    x.DisplayName,
                    x.RoutePattern,
                    Metadata = x.Metadata.Select(md =>
                    {
                        switch (md)
                        {
                            case PageRouteMetadata prm:
                                return (object)new
                                {
                                    prm.PageRoute,
                                    prm.RouteTemplate,
                                    Type = prm.GetType().Name
                                };
                            case PageActionDescriptor pad:
                                return (object)new
                                {
                                    pad.Id,
                                    pad.AreaName,
                                    pad.DisplayName,
                                    pad.ViewEnginePath,
                                    pad.RelativePath,
                                    Type = pad.GetType().Name
                                };
                            case RouteNameMetadata rnm:
                                return (object)new
                                {
                                    rnm.RouteName,
                                    Type = rnm.GetType().Name
                                };
                            case SuppressLinkGenerationMetadata slg:
                                return (object)new {
                                    slg.SuppressLinkGeneration,
                                    Type = slg.GetType().Name
                                };
                            default:
                                return (object)new{
                                    ToString = md.ToString(),
                                    Type = md.GetType().Name
                                };
                        }
                    })
                })
                .ToList();

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(endpoints), "application/json");
        }
    }
}
