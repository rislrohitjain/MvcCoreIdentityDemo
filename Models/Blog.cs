namespace MvcCoreIdentityDemo.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Identity User Link
        public string UserId { get; set; } = string.Empty;

        public List<BlogImage> Images { get; set; } = new();
        public List<Rating> Ratings { get; set; } = new();
    }
}
