using API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CreateAreaDto
    {
        [Required(ErrorMessage = "An area must have a name.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "An Area must be connected to a Stable.")]
        public int StableId { get; set; }
    }
}
