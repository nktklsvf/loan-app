using LoanApplication.Data;
using LoanApplication.Models;
using LoanApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanApplication.Controllers
{
    public class LoanController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;

        public LoanController(UserManager<User> userManager, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
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
        [Authorize]
        public ActionResult AddAction(int id)
        {
            AddLoanActionModel addLoanAction = new AddLoanActionModel();
            addLoanAction.LoanId = id;
            return View(addLoanAction);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAction(AddLoanActionModel obj)
        {
            if (ModelState.IsValid)
            {
                LoanAction loanAction = new LoanAction();
                User giverUser = _db.Users.FirstOrDefault(user => user.UserName == obj.GiverUserName);
                User takerUser = _db.Users.FirstOrDefault(user => user.UserName == obj.TakerUserName);
                Console.WriteLine(obj.LoanId);
                Loan loan = _db.Loans.Single(l => l.Id == obj.LoanId);
                if (loan != null)
                {
                    if (giverUser == null)
                    {
                        User user = new User();
                        user.UserName = obj.GiverUserName;
                        user.IsGhost = true;
                        user.PhoneNumber = "-";
                        var result = await _userManager.CreateAsync(user);
                        if (result.Succeeded)
                        {
                            giverUser = user;
                        }
                    }
                    if (takerUser == null)
                    {
                        User user = new User();
                        user.UserName = obj.TakerUserName;
                        user.IsGhost = true;
                        user.PhoneNumber = "-";
                        var result = await _userManager.CreateAsync(user);
                        if (result.Succeeded)
                        {
                            takerUser = user;
                        }
                    }
                    if (giverUser != null && takerUser != null)
                    {
                        loanAction.GiverUser = giverUser;
                        loanAction.TakerUser = takerUser;
                        loanAction.Purpose = obj.Purpose;
                        loanAction.Amount = obj.Amount;
                        loanAction.Loan = loan;
                        loanAction.CreatingTime = DateTime.Now;
                        _db.LoanActions.Add(loanAction);
                        _db.SaveChanges();
                        return Redirect("/Loan/Show/" + obj.LoanId);
                    }
                }
            }
            return View(obj);
        }
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AddLoanModel obj)
        {
            if (ModelState.IsValid)
            {
                Loan loan = new Loan();
                loan.Name = obj.Name;
                loan.Description = obj.Description;
                loan.CreatingTime = DateTime.Now;
                _db.Loans.Add(loan);
                _db.SaveChanges();
                return RedirectToAction("Index", "Loan");
            }
            return View();
        }


    }
}
