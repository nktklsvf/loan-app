using LoanApplication.Data;
using LoanApplication.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace LoanApplication.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;

        public LoanRepository(UserManager<User> userManager, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
        }
        public void Create(Loan item)
        {
            _db.Loans.Add(item);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            Loan loan = _db.Loans.Find(id);
            if (loan != null)
            {
                _db.Loans.Remove(loan);
                _db.SaveChanges();
            }
        }

        public Loan Get(int id)
        {
            return _db.Loans.Find(id);
        }

        public List<Loan> GetAll()
        {
            return _db.Set<Loan>().ToList();
        }
    }
}
