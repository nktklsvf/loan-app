﻿using LoanApplication.Data;
using LoanApplication.Models;
using LoanApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LoanApplication.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<User> _userManager;

        public SearchController(UserManager<User> userManager, ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        private SearchResultViewModel search(string text, string searchingFor, Expression<Func<User, bool>> condition)
        {
            List<User> users = _applicationDbContext.Users
            .Include(m => m.LoanActionAsGiver)
            .Include(m => m.LoanActionAsTaker)
            .Where(condition)
            .ToList();

            SearchResultViewModel searchResultViewModel = new SearchResultViewModel();
            searchResultViewModel.SearchingFor = searchingFor;
            searchResultViewModel.SearchInput = text;
            searchResultViewModel.Found = users.Select(m => AccountController.CreateAccountModelForUser(User, false, m)).ToList();

            return searchResultViewModel;
        }
        public IActionResult ByPhone([FromQuery(Name = "number")] string number)
        {
            return View("Result", search(number, "number", m => m.PhoneNumber.Contains(number)));
        }

        public IActionResult ByUsername([FromQuery(Name = "username")] string username)
        {
            return View("Result", search(username, "username", m => m.UserName.Contains(username)));
        }
    }
}
