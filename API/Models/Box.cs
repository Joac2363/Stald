using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Box
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(15)]
        public string Number {  get; set; } // is string since box number may be: 'E93' or similar.
        [Required]
        public int AreaId {  get; set; }
        [ForeignKey(nameof(AreaId))]
        public Area Area { get; set; }

        //TODO:
        // Add horse support (lol)
    }
}
