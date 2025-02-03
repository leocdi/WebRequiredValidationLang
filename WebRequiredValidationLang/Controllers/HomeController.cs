using System.Diagnostics;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebRequiredValidationLang.Models;
using WebRequiredValidationLang.Resources;

namespace WebRequiredValidationLang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<ValidationMessages> _localizer;


        public HomeController(ILogger<HomeController> logger, IStringLocalizer<ValidationMessages> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            var s = _localizer.GetString("Required");
            var x = _localizer.GetAllStrings();
            return View();
        }

        [HttpPost]
        public IActionResult Index(IndexViewModel vm)
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
    }
}
