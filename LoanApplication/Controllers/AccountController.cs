using LoanApplication.Data;
using LoanApplication.Models;
using LoanApplication.Repositories;
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
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<User> _signInManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
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
            User user = _userRepository.GetByUsername(username);

            bool isInContact = _userRepository.IsInContact(User.Identity.Name, username);
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
                            User ghostUser = _userRepository.GetGhostUser(user.UserName);
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
                                        _userRepository.UpdateUser(ghostUser);
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
            List<User> users = _userRepository.GetUserContacts(User.Identity.Name);
            contactListModel.contacts = users.Select(m => CreateAccountModelForUser(User, false, m)).ToList();
            return View(contactListModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddContact(string username)
        {
            _userRepository.AddUserContact(User.Identity.Name, username);
            return Redirect(Request.Headers.Referer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveContact(string username)
        {
            _userRepository.RemoveUserContact(User.Identity.Name, username);
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
