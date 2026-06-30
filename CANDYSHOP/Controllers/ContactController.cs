using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CANDYSHOP.Data;
using CANDYSHOP.Models;

namespace CANDYSHOP.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Static in-memory database to store timestamps of submissions per IP
        private static readonly ConcurrentDictionary<string, List<DateTime>> _requestStore = new();
        private static readonly object _lock = new();

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitMessage(string user_name, string user_email, string subject, string msg)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Apply sliding window rate limit check (max 3 messages in 5 minutes)
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                var list = _requestStore.GetOrAdd(ipAddress, _ => new List<DateTime>());
                
                // Keep only timestamps from the last 5 minutes
                list.RemoveAll(t => t < now.AddMinutes(-5));

                if (list.Count >= 3)
                {
                    return Json(new { type = "error", text = "Rate limit exceeded. You can only send 3 messages every 5 minutes." });
                }

                // Log this request timestamp
                list.Add(now);
            }

            if (string.IsNullOrWhiteSpace(user_name) || string.IsNullOrWhiteSpace(user_email) || string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { type = "error", text = "Please fill in all required fields." });
            }

            try
            {
                var contactMessage = new ContactMessage
                {
                    Name = user_name,
                    Email = user_email,
                    Subject = subject,
                    Message = msg,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                _context.ContactMessages.Add(contactMessage);
                await _context.SaveChangesAsync();

                return Json(new { type = "success", text = "Thank you! Your message has been sent successfully." });
            }
            catch (Exception)
            {
                return Json(new { type = "error", text = "An error occurred while sending your message. Please try again." });
            }
        }
    }
}
