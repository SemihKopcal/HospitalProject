using HospitalApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HospitalApp.Controllers
{
    //[Authorize(Roles = "Asistant, Professor, Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //if (User.IsInRole("Admin"))
            //{
                return View();
            //}
            //else
            //{
                // Eðer Admin deðilse, eriþim reddedildi mesajý veya yönlendirme yapabilirsiniz
            //    return RedirectToAction("Privacy", "Home");  // Ya da baþka bir sayfaya yönlendirebilirsiniz
            //}
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
