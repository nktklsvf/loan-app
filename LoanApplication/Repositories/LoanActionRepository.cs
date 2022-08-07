using LoanApplication.Data;
using LoanApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace LoanApplication.Repositories
{
    public class LoanActionRepository : ILoanActionRepository
    {
        private readonly ApplicationDbContext _db;

        public LoanActionRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public void Create(LoanAction item)
        {
            _db.LoanActions.Add(item);
            _db.SaveChanges();
        }

        public List<LoanAction> Get(int id)
        {
            return _db.LoanActions
                .Include(x => x.GiverUser)
                .Include(x => x.TakerUser)
                .Where(obj => obj.Loan.Id == id).ToList();
            
        }
    }
}
