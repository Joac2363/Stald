using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace API.Models
{
    [Index(nameof(StartDate))]
    [Index(nameof(EndDate))]
    public class Event
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? SeriesId { get; set; }
        public DateTime? RecurrenceId { get; set; }
        public int StableId { get; set; }
        [ForeignKey(nameof(StableId))]
        public Stable Stable { get; set; }
        public bool isOverride { get; set; }
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