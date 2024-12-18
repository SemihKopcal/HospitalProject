using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationHospital.Data;
using WebApplicationHospital.Models;

namespace HospitalApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProfessorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfessorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var professorRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Professor");

            if (professorRole == null)
            {
                return NotFound();
            }

            var professors = await _context.UserRoles
                .Where(ur => ur.RoleId == professorRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var professorUsers = await _context.Users
                .Include(u => u.Departments) // Include the Department navigation property
                .Where(u => professors.Contains(u.Id))
                .ToListAsync();

            var currentUser = await _userManager.GetUserAsync(User);
            bool isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            ViewData["IsAdmin"] = isAdmin;
            ViewData["Title"] = "Professor";

            return View(professorUsers);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Edit(string id)
        {
            var professor = await _context.Users.Include(u => u.Departments).FirstOrDefaultAsync(u => u.Id == id);
            if (professor == null)
            {
                return NotFound();
            }
            return View(professor);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var professorToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (professorToUpdate == null)
            {
                return NotFound();
            }

            professorToUpdate.Name = model.Name;
            professorToUpdate.Email = model.Email;
            professorToUpdate.PhoneNumber = model.PhoneNumber;
            professorToUpdate.DepartmentId = model.DepartmentId; // Corrected: Assign DepartmentId directly

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "The record was updated by another user.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(string id)
        {
            var professor = await _context.Users.Include(u => u.Departments).FirstOrDefaultAsync(u => u.Id == id);
            if (professor == null)
            {
                return NotFound();
            }
            return View(professor);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var professor = await _context.Users.FindAsync(id);
            if (professor != null)
            {
                _context.Users.Remove(professor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
