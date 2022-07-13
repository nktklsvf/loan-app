using LoanApplication.Data;
using System.ComponentModel.DataAnnotations;

namespace LoanApplication.Validations
{
    public class UniqueNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                ApplicationDbContext context = validationContext.GetService<ApplicationDbContext>();// (typeof(ApplicationDbContext));
                if (!context.Users.Any(user => user.Name == value.ToString()))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Username already exists");
        }
    }
}
