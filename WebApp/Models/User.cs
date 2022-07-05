using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Validations;

namespace WebApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [UniqueName]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        [NotMapped]
        [Compare("Password", ErrorMessage ="Password are not the same")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

    }
}