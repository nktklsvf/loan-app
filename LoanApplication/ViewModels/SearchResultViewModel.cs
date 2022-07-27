namespace LoanApplication.ViewModels
{
    public class SearchResultViewModel
    {
        public string SearchingFor { get; set; }
        public string SearchInput { get; set; }
        public List<AccountModel> Found { get; set; }
    }
}
