using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcCoreIdentityDemo.Data;
using MvcCoreIdentityDemo.Models;
using MvcCoreIdentityDemo.ViewModels;

namespace MvcCoreIdentityDemo.Controllers
{
    [Authorize] // Restrict entire controller to logged-in users
    public class AdminBlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminBlogsController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        // 1. READ (List)
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;
            var query = _context.Blog.Include(b => b.Images);

            var count = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            return View(items);
        }

        // 2. CREATE (GET)
        [HttpGet]
        public IActionResult Create() => View();

        // 3. CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var blog = new Blog
            {
                Title = model.Title,
                Summary = model.Summary,
                Description = model.Description,
                UserId = _userManager.GetUserId(User),
                CreatedAt = DateTime.Now
            };

            await HandleImageUploads(model, blog);

            _context.Add(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 4. EDIT (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var blog = await _context.Blog.FindAsync(id);
            if (blog == null) return NotFound();

            var model = new BlogCreateViewModel
            {
                Title = blog.Title,
                Summary = blog.Summary,
                Description = blog.Description
            };
            ViewBag.BlogId = id;
            return View(model);
        }

        // 5. EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogCreateViewModel model)
        {
            var blog = await _context.Blog.Include(b => b.Images).FirstOrDefaultAsync(x => x.Id == id);
            if (blog == null) return NotFound();

            blog.Title = model.Title;
            blog.Summary = model.Summary;
            blog.Description = model.Description;

            await HandleImageUploads(model, blog);

            _context.Update(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 6. DELETE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blog.FindAsync(id);
            if (blog != null) _context.Blog.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper Method for Images
        private async Task HandleImageUploads(BlogCreateViewModel model, Blog blog)
        {
            if (model.GalleryImages != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                foreach (var file in model.GalleryImages)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(folder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    blog.Images.Add(new BlogImage { ImagePath = "/uploads/" + fileName });
                }
            }
        }
    }
}