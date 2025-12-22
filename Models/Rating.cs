namespace MvcCoreIdentityDemo.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int Stars { get; set; } // 1-5
        public int BlogId { get; set; }
    }
}
