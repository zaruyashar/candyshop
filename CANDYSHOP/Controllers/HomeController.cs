using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CANDYSHOP.Data;
using CANDYSHOP.Models;

namespace CANDYSHOP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexViewModel
            {
                Products = await _context.Products.ToListAsync(),
                Testimonials = await _context.Testimonials.ToListAsync(),
                BlogPosts = await _context.BlogPosts.OrderByDescending(b => b.CreatedDate).Take(4).ToListAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> Blog()
        {
            var posts = await _context.BlogPosts.OrderByDescending(b => b.CreatedDate).ToListAsync();
            return View(posts);
        }

        public async Task<IActionResult> BlogPost(int id)
        {
            var post = await _context.BlogPosts.FirstOrDefaultAsync(b => b.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
