using HospitalApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalApp.Models;

namespace HospitalApp.Controllers
{
    public class ProfessorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfessorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 📘 Tüm kullanıcılar bu sayfayı görebilir
        public async Task<IActionResult> Index()
        {
            // 1️⃣ "Professor" rolünü bul
            var professorRole = await _context.Roles
                .Where(r => r.Name == "Professor")
                .FirstOrDefaultAsync();

            // Eğer "Professor" rolü yoksa hata döndür
            if (professorRole == null)
            {
                return NotFound();
            }

            // 2️⃣ UserRoles tablosundan "Professor" rolüne sahip kullanıcıların Id'lerini al
            var professors = await _context.UserRoles
                .Where(ur => ur.RoleId == professorRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // 3️⃣ Kullanıcılar tablosundan bu Id'lere sahip olan kullanıcıları çek
            var professorUsers = await _context.Users
                .Where(u => professors.Contains(u.Id))
                .ToListAsync();

            // 4️⃣ Şu anki giriş yapmış kullanıcıyı bul ve onun Admin olup olmadığını kontrol et
            var currentUser = await _userManager.GetUserAsync(User);
            bool isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            // 5️⃣ Admin kontrolü ViewData ile view'a gönderiliyor
            ViewData["IsAdmin"] = isAdmin;
            ViewData["Title"] = "Professor";

            // 6️⃣ Professor listesini view'a gönder
            return View(professorUsers);
        }

        // 📘 Sadece Admin kullanıcıların erişebileceği bölümler
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var professor = await _context.Users.FindAsync(id);
            if (professor == null)
            {
                return NotFound();
            }
            return View(professor);
        }

        [Authorize(Roles = "Admin")]
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
            professorToUpdate.Department = model.Department;

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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var professor = await _context.Users.FindAsync(id);
            if (professor == null)
            {
                return NotFound();
            }
            return View(professor);
        }

        [Authorize(Roles = "Admin")]
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
