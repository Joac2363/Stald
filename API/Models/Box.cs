using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Box
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Number is required.")]
        [MaxLength(15, ErrorMessage = "Number may not be longer than 15 characters.")]
        public string Number {  get; set; } // is string since box number may be: 'E93' or similar.
        [Required(ErrorMessage = "Box must be connected to an Area.")]
        public int AreaId {  get; set; }
        [ForeignKey(nameof(AreaId))]
        public Area Area { get; set; }
    }
}
