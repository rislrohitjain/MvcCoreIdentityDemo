using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MvcCoreIdentityDemo.Data;

namespace MvcCoreIdentityDemo.Controllers
{
    public class LandingController : Controller
    {

        private readonly IEnumerable<EndpointDataSource> _endpointSources;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<IdentityUser> _userManager;
        //public LandingController(IEnumerable<EndpointDataSource> endpointSources)
        //{
        //    _endpointSources = endpointSources;
        //}


        public LandingController(IEnumerable<EndpointDataSource> endpointSources,ApplicationDbContext context, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
        {
            _endpointSources = endpointSources; 
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult ShowRoutes()
        {
            var endpoints = _endpointSources
                .SelectMany(es => es.Endpoints)
                .OfType<RouteEndpoint>()
                .Select(e => new RouteInfo
                {
                    DisplayName = e.DisplayName,
                    Pattern = e.RoutePattern.RawText,
                    // Pulling metadata to see if it's an MVC Action
                    ActionName = e.Metadata.OfType<ActionDescriptor>().FirstOrDefault()?.RouteValues["action"],
                    ControllerName = e.Metadata.OfType<ActionDescriptor>().FirstOrDefault()?.RouteValues["controller"]
                })
                .OrderBy(r => r.Pattern)
                .ToList();

            return View(endpoints);
        }

        // Simple class to hold the data
        public class RouteInfo
        {
            public string? DisplayName { get; set; }
            public string? Pattern { get; set; }
            public string? ControllerName { get; set; }
            public string? ActionName { get; set; }
        }

        // GET: /Landing/Welcome
         
        public async Task<IActionResult> Welcome(int page = 1)
        {
            
            int pageSize = 3;
            var blogsQuery = _context.Blog.Include(b => b.Images).Include(b => b.Ratings);

            var count = await blogsQuery.CountAsync();
            var items = await blogsQuery.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            return View(items);
             
        }

        // GET: /Landing/Portfolio
        public IActionResult Portfolio()
        {
            return View();
        }

    }
}