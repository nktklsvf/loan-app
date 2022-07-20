using LoanApplication.Data;
using LoanApplication.Models;
using LoanApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
        }
        [HttpGet("/Account/Index/{username}")]
        public async Task<IActionResult> Index(string username)
        {
            User user = _applicationDbContext
                .Users
                .Include(x => x.LoanActionAsGiver)
                .ThenInclude(x => x.TakerUser)
                .Include(x => x.LoanActionAsTaker)
                .ThenInclude(x => x.GiverUser)
                .Where(x => x.UserName == username)
                .FirstOrDefault();

            AccountModel accountModel = new AccountModel();
            accountModel.Id = user.Id;
            accountModel.PhoneNumber = user.PhoneNumber;
            accountModel.UserName = username;

            int totalGave = 0;
            int totalTook = 0;

            foreach (LoanAction loanAction in user.LoanActionAsGiver)
            {
                totalGave += loanAction.Amount;
            }

            foreach (LoanAction loanAction in user.LoanActionAsTaker)
            {
                totalTook += loanAction.Amount;
            }

            accountModel.TotalGiver = totalGave;
            accountModel.TotalTaker = totalTook;

            if (User.Identity.IsAuthenticated)
            {
                if (_applicationDbContext.UserContacts.Include(m => m.ContactUser).Any(m => m.ContactUser.UserName == username))
                {
                    accountModel.HasAddContactButton = false;
                }
                else
                {
                    accountModel.HasAddContactButton = true;
                }
            } 
            else
            {
                accountModel.HasAddContactButton = false;
            }

            accountModel.LoanActionAsGiver = user.LoanActionAsGiver;
            accountModel.LoanActionAsTaker = user.LoanActionAsTaker;

            return View(accountModel);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.UserName, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [Authorize]
        [HttpGet]
        public IActionResult Contacts()
        {
            ContactListModel contactListModel = new ContactListModel();
            string[] contacts = _applicationDbContext.UserContacts
                            .Include(m => m.ContactUser)
                            .Include(m => m.User)
                            .Where(m => m.User.UserName == User.Identity.Name)
                            .Select(m => m.ContactUser.UserName)
                            .ToArray();
            contactListModel.contacts = contacts;
            return View(contactListModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddContact(string username)
        {
            User currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            UserContact userContact = new UserContact();
            userContact.UserId = currentUser.Id;
            userContact.ContactUserId = username;
            _applicationDbContext.Add(userContact);
            _applicationDbContext.SaveChanges();
            return Redirect(Request.Headers.Referer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveContact(string username)
        {
            UserContact userContact = _applicationDbContext.UserContacts
                            .Include(m => m.ContactUser)
                            .Include(m => m.User)
                            .Where(m => m.User.UserName == User.Identity.Name && m.ContactUserId == username)
                            .First();
            _applicationDbContext.Remove(userContact);
            _applicationDbContext.SaveChanges();
            return Redirect(Request.Headers.Referer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login or password");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
