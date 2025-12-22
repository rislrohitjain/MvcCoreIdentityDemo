using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcCoreIdentityDemo.Data;
using MvcCoreIdentityDemo.Models;
using MvcCoreIdentityDemo.ViewModels;

namespace MvcCoreIdentityDemo.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<IdentityUser> _userManager;

        public BlogsController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // GET: Public Blog List with Pagination
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 4;
            var blogsQuery = _context.Blog.Include(b => b.Images).Include(b => b.Ratings);

            var count = await blogsQuery.CountAsync();
            var items = await blogsQuery.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            return View(items);
        }
         
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(BlogCreateViewModel model)
        {
            var blog = new Blog
            {
                Title = model.Title,
                Summary = model.Summary,
                Description = model.Description,
                UserId = _userManager.GetUserId(User)
            };

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

            _context.Add(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddRating(int blogId, int stars)
        {
            var rating = new Rating
            {
                BlogId = blogId,
                Stars = stars
            };
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
