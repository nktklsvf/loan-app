namespace LoanApplication.ViewModels
{
    public class AddLoanActionModel
    {
        public int LoanId { get; set; }
        public string GiverUserName { get; set; }
        public string TakerUserName { get; set; }
        public int Amount { get; set; }
        public string Purpose { get; set; }
    }
}
