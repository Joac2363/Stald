using API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs
{
    public class CreateEventParticipantsDto
    {
        public ICollection<string> UserIds { get; set; } = new List<string>();
        public ICollection<int> TeamIds { get; set; } = new List<int>();
    }
}
