using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Test.Models;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public HomeController(
            IStringLocalizer<HomeController> localizer,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider
        )
        {
            _localizer = localizer;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }
        public IActionResult Index()
        {
            ViewBag.Title2 = _localizer["Title"];
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

    }
}