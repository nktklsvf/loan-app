using LoanApplication.Models;

namespace LoanApplication.Repositories
{
    public interface ILoanActionRepository
    {
        public void Create(LoanAction item);
        public List<LoanAction> Get(int id);
    }
}
