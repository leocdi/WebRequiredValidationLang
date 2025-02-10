using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using ValidationTraductionCore;
using WebRequiredValidationLang.Models;

namespace WebRequiredValidationLang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<ValidationMessages> _localizer;
        private readonly IServiceProvider _serviceProvider;

        public HomeController(ILogger<HomeController> logger, IStringLocalizer<ValidationMessages> localizer, IServiceProvider sp )
        {
            _logger = logger;
            _localizer = localizer;
            _serviceProvider = sp;
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

            var validation = ValiderManuellement(vm);

            foreach (var error in validation.Values.SelectMany(v => v.Errors))
            {
                Debug.WriteLine(error.ErrorMessage); // Les messages seront bien localisés
            }


            return View();
        }

        private ModelStateDictionary ValiderManuellement(object? model)
        {
            var ov = _serviceProvider.GetRequiredService<IObjectModelValidator>();
            var actionContext = new ActionContext();
            var validationState = new ValidationStateDictionary();
            ov.Validate(actionContext, validationState, "", model);
            return actionContext.ModelState;
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
