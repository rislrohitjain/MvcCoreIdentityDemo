using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcCoreIdentityDemo.Models;

namespace MvcCoreIdentityDemo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ContactRequest> ContactRequests { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<BlogImage> BlogImages { get; set; }
        public DbSet<Rating> Ratings { get; set; }

    }
}
