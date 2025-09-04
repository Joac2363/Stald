using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CreateStableDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }
    }
}
