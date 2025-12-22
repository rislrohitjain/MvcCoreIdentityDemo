namespace MvcCoreIdentityDemo.Models
{
    public class BlogImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public int BlogId { get; set; }
    }
}
