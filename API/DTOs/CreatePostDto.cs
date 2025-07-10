using API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CreatePostDto
    {
        [Required(ErrorMessage = "Post must be associated with a stable.")]
        public int StableId { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(50, ErrorMessage = "Title cannot be longer than 50 characters.")]
        public string Title { get; set; }
        public string Text { get; set; }
        

    }
}
