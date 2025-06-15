using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Memeber
    {
        [Key]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
        [Required(ErrorMessage = "Member must be associated with a stable.")]
        public int StableId { get; set; }
        [ForeignKey(nameof(StableId))]
        public Stable Stable { get; set; }
    }
}
