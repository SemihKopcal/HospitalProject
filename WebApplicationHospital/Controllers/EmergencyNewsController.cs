using Microsoft.AspNetCore.Mvc;
using WebApplicationHospital.Data;
using WebApplicationHospital.Models;
using MimeKit;
using MailKit.Net.Smtp;
using System.Linq;
using System.Threading.Tasks;

public class EmergencyNewsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SmtpSettings _smtpSettings;

    public EmergencyNewsController(ApplicationDbContext context, SmtpSettings smtpSettings)
    {
        _context = context;
        _smtpSettings = smtpSettings;
    }

    // Haberlerin listesi
    public async Task<IActionResult> Index()
    {
        var newsList = _context.EmergencyNews.OrderByDescending(n => n.CreatedAt).ToList();
        return View(newsList);
    }

    // Haber oluşturma (GET)
    public IActionResult Create()
    {
        return View();
    }

    // Haber oluşturma (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmergencyNews news)
    {
        if (ModelState.IsValid)
        {
            news.CreatedAt = DateTime.Now;
            _context.EmergencyNews.Add(news);
            await _context.SaveChangesAsync();

            // E-posta gönderme
            await SendEmailToTeam(news);

            return RedirectToAction(nameof(Index));
        }

        return View(news);
    }

    // Haber silme (DELETE)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var news = await _context.EmergencyNews.FindAsync(id);
        if (news == null)
        {
            return NotFound();
        }

        _context.EmergencyNews.Remove(news);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Ekibe E-posta Gönderme
    private async Task SendEmailToTeam(EmergencyNews news)
    {
        var teamEmails = _context.Users.Select(u => u.Email).ToList();

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
        emailMessage.Subject = $"Acil Durum Haberi: {news.Title}";

        emailMessage.Body = new TextPart("plain")
        {
            Text = news.Content
        };

        using (var smtpClient = new SmtpClient())
        {
            try
            {
                // SMTP bağlantısı
                await smtpClient.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);

                // Kimlik doğrulama
                await smtpClient.AuthenticateAsync(_smtpSettings.SenderEmail, _smtpSettings.SenderPassword);

                foreach (var email in teamEmails)
                {
                    emailMessage.To.Clear();
                    emailMessage.To.Add(new MailboxAddress("", email));

                    try
                    {
                        await smtpClient.SendAsync(emailMessage);
                        Console.WriteLine($"Email successfully sent to {email}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
                    }
                }

                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to SMTP server: {ex.Message}");
            }
        }
    }
}