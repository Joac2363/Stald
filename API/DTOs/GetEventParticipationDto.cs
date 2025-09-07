using API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs
{
    public class GetEventParticipationDto
    {
        public Guid EventId { get; set; }
        public ICollection<GetUserDto> Users { get; set; } = new List<GetUserDto>();
        public ICollection<GetTeamDto> Teams { get; set; } = new List<GetTeamDto>();
    }
}
