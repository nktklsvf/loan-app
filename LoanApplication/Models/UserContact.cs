namespace LoanApplication.Models
{
    public class UserContact
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ContactUserId { get; set; }
        public User User { get; set; }
        public User ContactUser { get; set; }
    }
}
