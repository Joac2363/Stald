using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Horse
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public int? StableId { get; set; }
        [ForeignKey(nameof(StableId))]]
        public Stable Stable { get; set; }
        [Required]
        public bool IsOwnedByStable { get; set; }
        public Guid? OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public User? Owner { get; set; }
        public int? BoxId { get; set; }
        [ForeignKey(nameof(BoxId))]
        public Box? Box { get; set; }
        public int? GreenAreaId { get; set; }
        [ForeignKey(nameof(GreenAreaId))]
        public Area? GreenArea {get;set;}

    }
}
