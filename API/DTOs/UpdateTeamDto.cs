using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UpdateTeamDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public ICollection<Guid> TeamUserIds { get; set; }
    }
}
