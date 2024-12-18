using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationHospital.Data;
using WebApplicationHospital.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class CalendarController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CalendarController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Calendar listesi
    public async Task<IActionResult> Index()
    {
        var calendars = await _context.Calendars.Include(c => c.Assignments).ToListAsync();
        Console.WriteLine($"Calendars Count: {calendars.Count}");

        return View(calendars);
    }

    // Asistan atama sayfasını gösterir
    public IActionResult AssignAssistant(string startDate, string endDate)
    {
        if (DateTime.TryParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStartDate) &&
            DateTime.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedEndDate))
        {
            var calendars = _context.Calendars
                .Where(c => c.StartDate.Date >= parsedStartDate.Date && c.EndDate.Date <= parsedEndDate.Date)
                .Include(c => c.Assignments)
                .ToList();

            if (calendars == null || !calendars.Any())
            {
                Console.WriteLine("No calendars found for the given date range.");
                return NotFound("No calendars found for the given date range.");
            }

            Console.WriteLine($"Found {calendars.Count} calendars for the given date range.");
            return View("AssignAssistant", calendars); // AssignAssistant HTML sayfasına yönlendirme
        }

        return BadRequest("Invalid date format."); // Hatalı tarih formatı hatası döndür
    }

    // Asistan atama işlemi
    [HttpPost]
    public async Task<IActionResult> AssignAssistant(DateTime startDate, DateTime endDate, int assistantId)
    {
        var calendars = await _context.Calendars
            .Where(c => c.StartDate.Date >= startDate.Date && c.EndDate.Date <= endDate.Date)
            .Include(c => c.Assignments)
            .ToListAsync();

        if (calendars == null || !calendars.Any())
        {
            return NotFound("No calendars found for the given date range.");
        }

        var assistant = await _userManager.FindByIdAsync(assistantId.ToString());
        if (assistant == null || !(await _userManager.IsInRoleAsync(assistant, "Assistant")))
        {
            return BadRequest("Invalid assistant user.");
        }

        foreach (var calendar in calendars)
        {
            var assignment = new Assignment
            {
                AssistantId = assistantId.ToString(),
                CalendarId = calendar.Id
            };

            _context.Assignments.Add(assignment);
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"Assistant with ID {assistantId} assigned to calendars between {startDate} and {endDate}.");

        return RedirectToAction("AssignAssistant", new
        {
            startDate = startDate.ToString("yyyy-MM-dd"),
            endDate = endDate.ToString("yyyy-MM-dd")
        });
    }
}