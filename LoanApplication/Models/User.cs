﻿using LoanApplication.Validations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LoanApplication.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public bool? IsGhost { get; set; }
        public virtual ICollection<LoanAction>? LoanActionAsGiver { get; set; }
        public virtual ICollection<LoanAction>? LoanActionAsTaker { get; set; }
        public virtual ICollection<UserContact>? UserContacts { get; set; }
        public virtual ICollection<UserContact>? UserAsContact { get; set; }
    }
}
