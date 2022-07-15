namespace LoanApplication.Models
{
    public class AddLoanAction
    {
        public int LoanId { get; set; }
        public String GiverUserName { get; set; }
        public String TakerUserName { get; set; }
        public int Amount { get; set; }
        public String Purpose { get; set; }
    }
}
