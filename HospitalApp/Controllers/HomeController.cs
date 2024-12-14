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
                // E�er Admin de�ilse, eri�im reddedildi mesaj� veya y�nlendirme yapabilirsiniz
            //    return RedirectToAction("Privacy", "Home");  // Ya da ba�ka bir sayfaya y�nlendirebilirsiniz
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
