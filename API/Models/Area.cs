using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Area
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "An Area must have a name.")]
        [MaxLength(50, ErrorMessage = "An Area name cannot be longer than 50 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "An Area must be connected to a Stable.")]
        public int StableId { get; set; }
        [ForeignKey(nameof(StableId))]
        public Stable Stable { get; set; }

        public ICollection<Box> Boxes { get; set; }


        // NOTICE:
        // When an area is deleted, all associated boxes, will be deleted by cascade
    }
}
