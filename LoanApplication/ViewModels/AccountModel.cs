using LoanApplication.Models;

namespace LoanApplication.ViewModels
{
    public class AccountModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalGiver { get; set; }
        public int TotalTaker { get; set; }
        public bool HasAddContactButton { get; set; }
        public ICollection<LoanAction>? LoanActionAsGiver { get; set; }
        public ICollection<LoanAction>? LoanActionAsTaker { get; set; }
    }
}
