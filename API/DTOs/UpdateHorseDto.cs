using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UpdateHorseDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public int? StableId { get; set; }
        [Required]
        public bool IsOwnedByStable { get; set; }
        public Guid? OwnerId { get; set; }
        public int? BoxId { get; set; }
        public int? GreenAreaId { get; set; }

    }
}
