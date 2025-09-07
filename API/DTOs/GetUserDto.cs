using API.Models;

namespace API.DTOs
{
    public class GetUserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
