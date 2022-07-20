using LoanApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoanApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LoanAction>()
                        .HasOne(m => m.GiverUser)
                        .WithMany(t => t.LoanActionAsGiver)
                        .HasForeignKey(m => m.GiverUserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<LoanAction>()
                        .HasOne(m => m.TakerUser)
                        .WithMany(t => t.LoanActionAsTaker)
                        .HasForeignKey(m => m.TakerUserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<UserContact>()
                        .HasOne(m => m.User)
                        .WithMany(t => t.UserContacts)
                        .HasForeignKey(m => m.UserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<UserContact>()
                        .HasOne(m => m.ContactUser)
                        .WithMany(t => t.UserAsContact)
                        .HasForeignKey(m => m.ContactUserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

        }

        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<LoanAction> LoanActions { get; set; }
        public virtual DbSet<UserContact> UserContacts { get; set; }
    }
}
