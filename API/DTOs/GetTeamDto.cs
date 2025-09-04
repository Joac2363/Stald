using API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs
{
    public class GetTeamDto
    {
        public int Id { get; set; }
        public int StableId { get; set; }
        public string Name { get; set; }
        public ICollection<GetUserDto> TeamUsers { get; set; }
    }
}
