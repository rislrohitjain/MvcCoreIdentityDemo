using System.ComponentModel.DataAnnotations;


namespace MvcCoreIdentityDemo.Models
{
    public class ContactRequest
    {
        public int Id { get; set; }


        [Required]
        public string Name { get; set; }


        [Required, EmailAddress]
        public string Email { get; set; }


        [Required]
        public string Mobile { get; set; }


        [Required]
        public string Message { get; set; }


        public string? FilePath { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}