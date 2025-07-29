using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Stable
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Stable name is required.")]
        [StringLength(100, ErrorMessage = "Stable name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot be longer than 200 characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Owner is required")]
        public string OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }

        ICollection<Area> Areas { get; set; }
    }

}
