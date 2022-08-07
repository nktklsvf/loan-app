using LoanApplication.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace LoanApplication.Repositories
{
    public interface IUserRepository
    {
        public Task<IdentityResult> Create(User item);

        public User FirstOrDefault(Expression<Func<User, bool>> source);
        public User GetByUsername(string username);
        public bool IsInContact(string firstUsername, string secondUsername);
        public User GetGhostUser(string username);
        public void UpdateUser(User user);
        public void AddUserContact(string userFor, string userAdded);
        public void RemoveUserContact(string userFor, string userRemoved);
        public List<User> GetUserContacts(string username);
    }
}
