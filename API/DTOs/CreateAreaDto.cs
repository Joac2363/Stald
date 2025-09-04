using API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CreateAreaDto
    {
        public string Name { get; set; }
        [Required]
        public int StableId { get; set; }
    }
}
