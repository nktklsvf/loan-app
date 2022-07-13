using LoanApplication.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LoanApplication.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [UniqueName]
        public string? Name { get; set; }
        [Required]
        public string? Password { get; set; }
        [NotMapped]
        [Compare("Password", ErrorMessage = "Passwords are not the same")]
        public string? ConfirmPassword { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        public virtual ICollection<LoanAction>? LoanActionAsGiver { get; set; }
        public virtual ICollection<LoanAction>? LoanActionAsTaker { get; set; }

    }
}
