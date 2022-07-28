using LoanApplication.Data;
using LoanApplication.Models;
using LoanApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        public static AccountModel CreateAccountModelForUser(ClaimsPrincipal currentUser, bool isInContacts, User user)
        {
            AccountModel accountModel = new AccountModel();
            accountModel.Id = user.Id;
            accountModel.PhoneNumber = user.PhoneNumber;
            accountModel.UserName = user.UserName;

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

            if (currentUser.Identity.IsAuthenticated)
            {
                if (isInContacts)
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

            return accountModel;
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

            bool isInContact = _applicationDbContext.UserContacts.Include(m => m.ContactUser).Any(m => m.ContactUser.UserName == user.UserName);
            AccountModel accountModel = CreateAccountModelForUser(User, isInContact, user);
            return View(accountModel);
        }
        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel registerViewModel = new RegisterViewModel();
            registerViewModel.DisplayGhostQuestion = false;
            registerViewModel.ConfirmGhost = false;
            return View(registerViewModel);
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
                    return RedirectToAction("Index", "Loan");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "DuplicateUserName")
                        {
                            User ghostUser = _applicationDbContext.Users.Where(m => m.UserName == user.UserName && m.IsGhost == true).FirstOrDefault();
                            if (ghostUser != null)
                            {
                                if (model.DisplayGhostQuestion == false)
                                {
                                    model.DisplayGhostQuestion = true;
                                    return View(model);
                                }
                                else
                                {
                                    if (model.ConfirmGhost)
                                    {
                                        ghostUser.PhoneNumber = model.PhoneNumber;
                                        ghostUser.IsGhost = false;
                                        _applicationDbContext.Update(ghostUser);
                                        await _userManager.AddPasswordAsync(ghostUser, model.Password);
                                        await _userManager.SetEmailAsync(ghostUser, model.Email);
                                        await _signInManager.SignInAsync(ghostUser, false);
                                        return RedirectToAction("Index", "Loan");
                                    }
                                }
                            }
                        }
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
            List<User> users = _applicationDbContext.UserContacts
                            .Include(m => m.ContactUser)
                            .Include(m => m.ContactUser.LoanActionAsGiver)
                            .Include(m => m.ContactUser.LoanActionAsTaker)
                            .Include(m => m.User)
                            .Where(m => m.User.UserName == User.Identity.Name)
                            .Select(m => m.ContactUser)
                            .ToList();

            contactListModel.contacts = users.Select(m => CreateAccountModelForUser(User, false, m)).ToList();
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
                    return RedirectToAction("Index", "Loan");
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
            return RedirectToAction("Index", "Loan");
        }
    }
}
