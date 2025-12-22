using Microsoft.AspNetCore.Mvc;
using MvcCoreIdentityDemo.Data;
using MvcCoreIdentityDemo.Models;


namespace MvcCoreIdentityDemo.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;


        public ContactController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactRequest model, IFormFile? Attachment)
        {
            if (ModelState.IsValid)
            {
                if (Attachment != null)
                {
                    string uploadDir = Path.Combine(_env.WebRootPath, "uploads/contact");
                    Directory.CreateDirectory(uploadDir);


                    string fileName = Guid.NewGuid() + Path.GetExtension(Attachment.FileName);
                    string filePath = Path.Combine(uploadDir, fileName);


                    using var stream = new FileStream(filePath, FileMode.Create);
                    await Attachment.CopyToAsync(stream);


                    model.FilePath = "/uploads/contact/" + fileName;
                }


                _context.ContactRequests.Add(model);
                await _context.SaveChangesAsync();


                TempData["success"] = "Your request has been submitted successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}