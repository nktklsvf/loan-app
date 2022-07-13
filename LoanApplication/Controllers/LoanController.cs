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
