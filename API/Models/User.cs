namespace API.Models
{
    using Microsoft.AspNetCore.Identity;

    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Member Member { get; set; }
        public ICollection<Team> TeamUsers { get; set; }
    }
}
