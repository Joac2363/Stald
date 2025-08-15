using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UpdateStableDto
    {
        [Key]
        [Required(ErrorMessage = "A stable must have an id.")]
        public int Id { get; set; }
        [StringLength(100, ErrorMessage = "Stable name cannot be longer than 100 characters.")]
        public string? Name { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot be longer than 200 characters.")]
        public string? Address { get; set; }
    }
}
