namespace LoanApplication.Models
{
    public class LoanView
    {
        public LoanView(Loan loan, List<LoanAction> actions)
        {
            Loan = loan;
            Actions = actions;
        }
        public Loan Loan { get; set; }
        public List<LoanAction> Actions { get; set; }
    }
}
