using HospitalApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class DepartmentController : Controller
{
    private readonly ApplicationDbContext _context;

    public DepartmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Kullanıcıları departmana göre grupla, ancak Departmanı null veya boş olmayanları seç
        var departmentsWithUsers = await _context.Users
            .Where(u => !string.IsNullOrEmpty(u.Department))  // Departmanı boş olmayanları seç
            .GroupBy(u => u.Department) // Departman adına göre gruplama
            .Select(group => new
            {
                DepartmentName = group.Key,
                Users = group.ToList()  // Grup içindeki kullanıcıları listele
            })
            .ToListAsync();

        // Görüntü için departmanları model olarak gönderiyoruz
        return View(departmentsWithUsers);
    }

}
