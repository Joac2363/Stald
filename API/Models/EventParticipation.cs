using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class EventParticipation
    {
        [Key]
        [Required]
        public Guid EventId { get; set; }
        [ForeignKey(nameof(EventId))]
        public Event Event { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
