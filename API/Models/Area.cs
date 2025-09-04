using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Area
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public int StableId { get; set; }
        [ForeignKey(nameof(StableId))]
        public Stable Stable { get; set; }

        public ICollection<Box> Boxes { get; set; }


        // NOTICE:
        // When an area is deleted, all associated boxes, will be deleted by cascade
    }
}
