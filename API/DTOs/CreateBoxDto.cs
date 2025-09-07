using API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CreateBoxDto
    {
        [Required]
        [MaxLength(15)]
        public string Number { get; set; } // is string since box number may be: 'E93' or similar.
    }
}
