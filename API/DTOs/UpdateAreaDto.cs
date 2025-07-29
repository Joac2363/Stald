using API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UpdateAreaDto
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "An Area must have a name.")]
        [MaxLength(50, ErrorMessage = "An Area name cannot be longer than 50 characters.")]
        public string Name { get; set; }
    }
}
