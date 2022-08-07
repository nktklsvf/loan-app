using LoanApplication.Data;
using LoanApplication.Models;
using LoanApplication.Repositories;
using LoanApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace LoanApplication.Controllers
{
    public class LoanController : Controller
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanActionRepository _loanActionRepository;
        private readonly IUserRepository _userRepository;

        public LoanController(ILoanRepository loanRepository, ILoanActionRepository loanActionRepository, IUserRepository userRepository)
        {
            _loanActionRepository = loanActionRepository;
            _loanRepository = loanRepository;
            _userRepository = userRepository;

        }
        public IActionResult Index()
        {
            return View(_loanRepository.GetAll());
        }
        public ActionResult Show(int id)
        {
            Loan loan = _loanRepository.Get(id);

            List<LoanAction> actions = _loanActionRepository.Get(id);
            LoanView loanView = new LoanView(loan, actions);
            return View(loanView);
        }
        public ActionResult Remove(int id)
        {
            _loanRepository.Delete(id);
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
                User giverUser = _userRepository.FirstOrDefault(user => user.UserName == obj.GiverUserName);
                User takerUser = _userRepository.FirstOrDefault(user => user.UserName == obj.TakerUserName);
                Console.WriteLine(obj.LoanId);
                Loan loan = _loanRepository.Get(obj.LoanId);
                if (loan != null)
                {
                    if (giverUser == null)
                    {
                        User user = new User();
                        user.UserName = obj.GiverUserName;
                        user.IsGhost = true;
                        user.PhoneNumber = "-";
                        IdentityResult result = await _userRepository.Create(user);
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
                        IdentityResult result = await _userRepository.Create(user);
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
                        _loanActionRepository.Create(loanAction);
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
                _loanRepository.Create(loan);
                return RedirectToAction("Index", "Loan");
            }
            return View();
        }


    }
}
