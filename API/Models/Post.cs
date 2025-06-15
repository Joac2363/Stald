using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Post must be associated with a stable.")]
        public int StableId { get; set; }
        [ForeignKey(nameof(StableId))]
        public Stable Stable { get; set; }
        
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(50, ErrorMessage = "Title cannot be longer than 50 characters.")]
        public string Title { get; set; }
        public string Text { get; set; }
        [Required(ErrorMessage = "Creation date is required.")]
        public DateTime CreatedDate { get; set; }
        [Required(ErrorMessage = "User (creator) is required.")]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public ICollection<Media> MediaItems { get; set; }

    }
}
