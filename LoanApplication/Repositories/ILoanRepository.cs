using LoanApplication.Models;

namespace LoanApplication.Repositories
{
    public interface ILoanRepository
    {
        public void Create(Loan item);

        public void Delete(int id);

        public Loan Get(int id);

        public List<Loan> GetAll();
    }
}
