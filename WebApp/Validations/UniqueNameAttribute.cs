using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebApp.Data;

namespace WebApp.Validations
{
    public class UniqueNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
                if (!context.Users.Any(user => user.Name == value.ToString()))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Username already exists");
        }
    }
}