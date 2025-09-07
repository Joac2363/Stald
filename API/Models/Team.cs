using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int StableId { get; set; }
        [Required]
        [ForeignKey(nameof(StableId))]
        public Stable Stable { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public ICollection<User> TeamUsers { get; set; } = new List<User>();
        public ICollection<EventParticipation> EventParticipations { get; set; } = new List<EventParticipation>();

    }
}
