using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplicationHospital.Data;
using WebApplicationHospital.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationHospital.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // [HttpGet] Metodu: Randevu Al Sayfasını Gösterir
        [HttpGet]
        public IActionResult Book(int id)
        {
            ViewBag.CalendarId = id;
            return View();
        }

        // [HttpPost] Metodu: Randevu Al Formunu İşler
        [HttpPost]
        public async Task<IActionResult> BookPost(int calendarId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var assignment = new Assignment
            {
                CalendarId = calendarId,
                AssistantId = user.Id
            };

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Index(int? calendarId)
        {
            if (calendarId == null)
            {
                return NotFound("Calendar ID not provided.");
            }

            var calendar = await _context.Calendars
                .Where(c => c.Id == calendarId)
                .Include(c => c.Assignments) // Eğer ilişkili veriler varsa
                .FirstOrDefaultAsync();

            if (calendar == null)
            {
                return NotFound("Calendar not found.");
            }

            return View(calendar);
        }
    }
}