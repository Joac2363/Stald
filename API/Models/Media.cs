using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Media
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Filename is required.")]
        [StringLength(25, ErrorMessage = "Filename cannot be longer than 25 characters.")]
        public string Filename { get; set; }
        [Required(ErrorMessage = "Content type is required.")]
        [StringLength(15, ErrorMessage = "Content type cannot be longer than 15 characters.")]
        public string ContentType { get; set; }
        [Required(ErrorMessage = "File path is required.")]
        [StringLength(150, ErrorMessage = "File path cannot be longer than 150 characters.")]
        public string FilePath { get; set; }
        [Required(ErrorMessage = "Media must be associated with a post.")]
        public int PostId { get; set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }
    }
}
