using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int StableId { get; set; }
        [ForeignKey(nameof(StableId))]
        public Stable Stable { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        public string Text { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public ICollection<Media> MediaItems { get; set; }

    }
}
