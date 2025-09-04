using API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CreatePostDto
    {
        [Required]
        public int StableId { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        public string Text { get; set; }
        

    }
}
