using API.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs
{
    public class GetHorseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? StableId { get; set; }
        public bool IsOwnedByStable { get; set; }
        public GetUserDto? Owner { get; set; }
        public int? BoxId { get; set; }
        public string? BoxNumber { get; set; }
        public int? GreenAreaId { get; set; }
        public string? GreenAreaName { get; set; }

    }
}
