using API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs
{
    public class EventDto
    {
        [Key]
        public Guid? Id { get; set; }

        public Guid? SeriesId { get; set; }
        public DateTime? OriginalStartDate { get; set; }
        [Required]
        public bool isOverride { get; set; }
        [Required]
        public bool isRecurring { get; set; }
        public string? RecurrenceRule { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
    }
}
