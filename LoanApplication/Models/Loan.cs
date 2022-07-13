using System.ComponentModel.DataAnnotations;

namespace LoanApplication.Models
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
