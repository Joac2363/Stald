using API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UpdateAreaDto
    {
        [Key]
        [Required(ErrorMessage = "An area must have an id.")]
        public int Id { get; set; }
        [MaxLength(50, ErrorMessage = "An Area name cannot be longer than 50 characters.")]
        public string? Name { get; set; }
    }
}
