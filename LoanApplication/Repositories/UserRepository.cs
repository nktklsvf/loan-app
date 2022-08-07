using LoanApplication.Data;
using LoanApplication.Models;
using LoanApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LoanApplication.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IdentityResult> Create(User item)
        {
            return await _userManager.CreateAsync(item);
        }

        public User FirstOrDefault(Expression<Func<User, bool>> source)
        {
            return _db.Users.FirstOrDefault(source);
        }

        public User GetByUsername(string username)
        {
            return _db
                .Users
                .Include(x => x.LoanActionAsGiver)
                .ThenInclude(x => x.TakerUser)
                .Include(x => x.LoanActionAsTaker)
                .ThenInclude(x => x.GiverUser)
                .Where(x => x.UserName == username)
                .FirstOrDefault();
        }

        public bool IsInContact(string firstUsername, string secondUsername)
        {
            return _db.UserContacts
                .Include(m => m.ContactUser)
                .Include(m => m.User)
                .Any(m => m.User.UserName == firstUsername && m.ContactUser.UserName == secondUsername);
        }
        public User GetGhostUser(string username)
        {
            return _db.Users.Where(m => m.UserName == username && m.IsGhost == true).FirstOrDefault();
        }

        public void UpdateUser(User user)
        {
            _db.Update(user);
        }

        public void AddUserContact(string userFor, string userAdded)
        {
            User currentUser = _db.Users.Where(m => m.UserName == userFor).FirstOrDefault();//await _userManager.FindByNameAsync(userFor);
            if (currentUser != null)
            {
                UserContact userContact = new UserContact();
                userContact.UserId = currentUser.Id;
                userContact.ContactUserId = userAdded;

                _db.Add(userContact);
                _db.SaveChanges();
            }
        }

        public void RemoveUserContact(string userFor, string userRemoved)
        {
            UserContact userContact = _db.UserContacts
                            .Include(m => m.ContactUser)
                            .Include(m => m.User)
                            .Where(m => m.User.UserName == userFor && m.ContactUserId == userRemoved)
                            .First();
            _db.Remove(userContact);
            _db.SaveChanges();
        }

        public List<User> GetUserContacts(string username)
        {
            List<User> users = _db.UserContacts
                            .Include(m => m.ContactUser)
                            .Include(m => m.ContactUser.LoanActionAsGiver)
                            .Include(m => m.ContactUser.LoanActionAsTaker)
                            .Include(m => m.User)
                            .Where(m => m.User.UserName == username)
                            .Select(m => m.ContactUser)
                            .ToList();

            return users;
        }
    }
}
