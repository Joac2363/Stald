using API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs
{
    public class GetAreaDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StableId { get; set; }
        public bool IsGreenArea { get; set; }
        public ICollection<Box> Boxes { get; set; }
    }
}
