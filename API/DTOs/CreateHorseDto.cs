using API.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs
{
    public class CreateHorseDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public bool IsOwnedByStable { get; set; }
        public Guid? OwnerId { get; set; }
        public int? BoxId { get; set; }
        public int? GreenAreaId { get; set; }

    }
}
