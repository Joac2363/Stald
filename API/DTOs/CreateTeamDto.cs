using API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs
{
    public class CreateTeamDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public ICollection<Guid> TeamUserIds { get; set; }
    }
}
