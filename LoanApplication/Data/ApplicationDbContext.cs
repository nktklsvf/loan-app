using LoanApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace LoanApplication.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<LoanAction> LoanActions { get; set; }
    }
}
