using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanApplication.Models
{
    public class LoanAction
    {
        [Key]
        public int Id { get; set; }
        public Loan Loan { get; set; }
        public String GiverUserId { get; set; }
        public String TakerUserId { get; set; }
        public String Purpose { get; set; }
        public int Amount { get; set; }
        public User GiverUser { get; set; }
        public User TakerUser { get; set; }
        public DateTime CreatingTime { get; set; }
    }
}
