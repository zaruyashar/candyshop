using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CANDYSHOP.Data;
using CANDYSHOP.Models;

namespace CANDYSHOP.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action filter to inject unread messages badge count into all admin views
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            ViewBag.UnreadMessagesCount = _context.ContactMessages.Count(m => !m.IsRead);
            base.OnActionExecuting(context);
        }

        // GET: Admin (Dashboard)
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";
            
            ViewBag.ProductCount = await _context.Products.CountAsync();
            ViewBag.BlogPostCount = await _context.BlogPosts.CountAsync();
            ViewBag.TestimonialCount = await _context.Testimonials.CountAsync();
            ViewBag.MessageCount = await _context.ContactMessages.CountAsync();
            ViewBag.UnreadCount = await _context.ContactMessages.CountAsync(m => !m.IsRead);

            return View("Dashboard");
        }

        // ==========================================
        // PRODUCTS CRUD
        // ==========================================

        // GET: Admin/Products
        public async Task<IActionResult> Products(string? q)
        {
            ViewData["Title"] = "Products";
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(q) || p.Category.ToLower().Contains(q));
            }

            var products = await query.ToListAsync();
            ViewBag.SearchQuery = q;
            return View(products);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Add Product";
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Products));
            }
            ViewData["Title"] = "Add Product";
            return View(product);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit Product";
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(p => p.Id == product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Products));
            }
            ViewData["Title"] = "Edit Product";
            return View(product);
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ==========================================
        // BLOG POSTS CRUD
        // ==========================================

        // GET: Admin/BlogPosts
        public async Task<IActionResult> BlogPosts(string? q)
        {
            ViewData["Title"] = "Blog Posts";
            var query = _context.BlogPosts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(q) || b.Summary.ToLower().Contains(q));
            }

            var posts = await query.OrderByDescending(b => b.CreatedDate).ToListAsync();
            ViewBag.SearchQuery = q;
            return View(posts);
        }

        // GET: Admin/BlogPostCreate
        public IActionResult BlogPostCreate()
        {
            ViewData["Title"] = "Add Blog Post";
            return View();
        }

        // POST: Admin/BlogPostCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlogPostCreate(BlogPost post)
        {
            if (ModelState.IsValid)
            {
                post.CreatedDate = DateTime.UtcNow;
                _context.BlogPosts.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(BlogPosts));
            }
            ViewData["Title"] = "Add Blog Post";
            return View(post);
        }

        // GET: Admin/BlogPostEdit/5
        public async Task<IActionResult> BlogPostEdit(int id)
        {
            ViewData["Title"] = "Edit Blog Post";
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Admin/BlogPostEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlogPostEdit(int id, BlogPost post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.BlogPosts.Any(b => b.Id == post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(BlogPosts));
            }
            ViewData["Title"] = "Edit Blog Post";
            return View(post);
        }

        // POST: Admin/BlogPostDelete/5
        [HttpPost]
        public async Task<IActionResult> BlogPostDelete(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                return Json(new { success = false, message = "Blog post not found." });
            }

            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ==========================================
        // TESTIMONIALS CRUD
        // ==========================================

        // GET: Admin/Testimonials
        public async Task<IActionResult> Testimonials(string? q)
        {
            ViewData["Title"] = "Testimonials";
            var query = _context.Testimonials.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToLower();
                query = query.Where(t => t.ClientName.ToLower().Contains(q) || t.Comment.ToLower().Contains(q));
            }

            var testimonials = await query.ToListAsync();
            ViewBag.SearchQuery = q;
            return View(testimonials);
        }

        // GET: Admin/TestimonialCreate
        public IActionResult TestimonialCreate()
        {
            ViewData["Title"] = "Add Testimonial";
            return View();
        }

        // POST: Admin/TestimonialCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestimonialCreate(Testimonial testimonial)
        {
            if (ModelState.IsValid)
            {
                _context.Testimonials.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Testimonials));
            }
            ViewData["Title"] = "Add Testimonial";
            return View(testimonial);
        }

        // GET: Admin/TestimonialEdit/5
        public async Task<IActionResult> TestimonialEdit(int id)
        {
            ViewData["Title"] = "Edit Testimonial";
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            return View(testimonial);
        }

        // POST: Admin/TestimonialEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestimonialEdit(int id, Testimonial testimonial)
        {
            if (id != testimonial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testimonial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Testimonials.Any(t => t.Id == testimonial.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Testimonials));
            }
            ViewData["Title"] = "Edit Testimonial";
            return View(testimonial);
        }

        // POST: Admin/TestimonialDelete/5
        [HttpPost]
        public async Task<IActionResult> TestimonialDelete(int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return Json(new { success = false, message = "Testimonial not found." });
            }

            _context.Testimonials.Remove(testimonial);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ==========================================
        // MESSAGES MANAGEMENT
        // ==========================================

        // GET: Admin/Messages
        public async Task<IActionResult> Messages()
        {
            ViewData["Title"] = "Messages";
            var messages = await _context.ContactMessages.OrderByDescending(m => m.CreatedAt).ToListAsync();
            return View(messages);
        }

        // GET: Admin/MessageDetails/5
        public async Task<IActionResult> MessageDetails(int id)
        {
            ViewData["Title"] = "Message Details";
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg == null)
            {
                return NotFound();
            }

            if (!msg.IsRead)
            {
                msg.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(msg);
        }

        // POST: Admin/MarkAsRead/5
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg == null)
            {
                return Json(new { success = false });
            }

            msg.IsRead = true;
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // POST: Admin/DeleteMessage/5
        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg == null)
            {
                return Json(new { success = false, message = "Message not found." });
            }

            _context.ContactMessages.Remove(msg);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
