using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanApplication.Models
{
    public class LoanAction
    {
        [Key]
        public int Id { get; set; }
        public Loan Loan { get; set; }
        public int GiverUserId { get; set; }
        public int TakerUserId { get; set; }
        public String Purpose { get; set; }
        public int Amount { get; set; }
        public virtual User GiverUser { get; set; }
        public virtual User TakerUser { get; set; }
    }
}
