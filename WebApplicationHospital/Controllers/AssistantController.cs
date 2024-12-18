using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationHospital.Data;
using WebApplicationHospital.Models;

namespace HospitalApp.Controllers
{
    public class AssistantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AssistantController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // View assistant list for everyone
        public async Task<IActionResult> Index()
        {
            var assistantRole = await _context.Roles
                .Where(r => r.Name == "Assistant")
                .FirstOrDefaultAsync();

            if (assistantRole == null)
            {
                return NotFound();
            }

            var assistants = await _context.UserRoles
                .Where(ur => ur.RoleId == assistantRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var assistantUsers = await _context.Users
                .Where(u => assistants.Contains(u.Id))
                .ToListAsync();

            // Check if the current user is an Admin
            var currentUser = await _userManager.GetUserAsync(User);
            bool isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            ViewData["IsAdmin"] = isAdmin; // Pass this to the view
            ViewData["Title"] = "Assistant";

            return View(assistantUsers);
        }



        // GET: Edit
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assistant = await _userManager.FindByIdAsync(id);
            if (assistant == null)
            {
                return NotFound();
            }

            // Kullanıcıyı viewmodel'e veya direkt olarak kullanıcıyı model olarak gönderiyoruz
            return View(assistant);  // View'da ApplicationUser modelini kullanıyoruz
        }

        // POST: Edit
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, ApplicationUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var userToUpdate = await _userManager.FindByIdAsync(id); // Bu satırda doğru kullanıcıyı alıyoruz

            if (userToUpdate == null)
            {
                return NotFound();
            }

            // Kullanıcı verilerini güncelle
            userToUpdate.Name = model.Name;
            userToUpdate.Email = model.Email;
            userToUpdate.PhoneNumber = model.PhoneNumber;

            try
            {
                var result = await _userManager.UpdateAsync(userToUpdate);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Eğer güncelleme işlemi başarısız olursa, hata mesajlarını ekliyoruz
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "The record was updated by another user.");
                return View(model);
            }

            return View(model);  // Eğer işlem başarısızsa, güncellenmiş modelle geri döneriz
        }


        // GET /Assistant/Delete/{id}
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var assistant = await _context.Users.FindAsync(id);
            if (assistant == null)
            {
                return NotFound(); // Kullanıcı bulunamadıysa hata döner
            }
            return View(assistant); // Kullanıcı bilgilerini Delete sayfasında gösterir
        }


        // POST /Assistant/Delete/{id} -> Silme işlemi yapılır
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var assistant = await _context.Users.FindAsync(id);
            if (assistant == null)
            {
                return NotFound(); // Kullanıcı bulunamadıysa hata döner
            }

            _context.Users.Remove(assistant); // Kullanıcıyı sileriz
            await _context.SaveChangesAsync(); // Değişiklikleri kaydederiz

            return RedirectToAction(nameof(Index)); // Silme başarılıysa listeye yönlendirir
        }

    }
}
