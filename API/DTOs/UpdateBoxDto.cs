using API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UpdateBoxDto
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [MaxLength(15, ErrorMessage = "Number may not be longer than 15 characters.")]
        public string? Number { get; set; } // is string since box number may be: 'E93' or similar.
        public int? AreaId { get; set; }
    }
}
