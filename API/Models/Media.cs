using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Media
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(25)]
        public string Filename { get; set; }
        [Required]
        [StringLength(15)]
        public string ContentType { get; set; }
        [Required]
        [StringLength(150)]
        public string FilePath { get; set; }
        [Required]
        public int PostId { get; set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }
    }
}
