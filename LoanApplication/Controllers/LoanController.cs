using LoanApplication.Data;
using LoanApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanApplication.Controllers
{
    public class LoanController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LoanController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.Set<Loan>().ToList());
        }
        public ActionResult Show(int id)
        {
            Loan loan = _db.Loans.Find(id);
            List<LoanAction> actions = _db.LoanActions
                .Include(x => x.GiverUser)
                .Include(x => x.TakerUser)
                .Where(obj => obj.Loan.Id == id).ToList();
            LoanView loanView = new LoanView(loan, actions);
            return View(loanView);
        }
        public ActionResult Remove(int id)
        {
            Console.WriteLine(id);
            Loan loan = _db.Loans.Find(id);
            if (loan != null)
            {
                _db.Loans.Remove(loan);
                _db.SaveChanges();
            }
            return Redirect("/Loan");
        }
        public ActionResult AddAction(int id)
        {
            AddLoanAction addLoanAction = new AddLoanAction();
            addLoanAction.LoanId = id;
            return View(addLoanAction);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAction(AddLoanAction obj)
        {
            if (ModelState.IsValid)
            {
                LoanAction loanAction = new LoanAction();
                User giverUser = _db.Users.Single(user => user.Name == obj.GiverUserName);
                User takerUser = _db.Users.Single(user => user.Name == obj.TakerUserName);
                Console.WriteLine(obj.LoanId);
                Loan loan = _db.Loans.Single(l => l.Id == obj.LoanId);
                if (giverUser != null && takerUser != null && loan != null)
                {
                    loanAction.GiverUser = giverUser;
                    loanAction.TakerUser = takerUser;
                    loanAction.Purpose = obj.Purpose;
                    loanAction.Amount = obj.Amount;
                    loanAction.Loan = loan;
                    _db.LoanActions.Add(loanAction);
                    _db.SaveChanges();
                    return Redirect("/Loan/Show/" + obj.LoanId);
                }
            }
            return View(obj);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Loan obj)
        {
            if (ModelState.IsValid)
            {
                _db.Loans.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Loan");
            }
            return View();
        }


    }
}
