namespace MvcCoreIdentityDemo.ViewModels
{
    public class BlogCreateViewModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public List<IFormFile> GalleryImages { get; set; }
    }
}
