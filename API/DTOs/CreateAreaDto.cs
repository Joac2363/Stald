using API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CreateAreaDto
    {
        public string Name { get; set; }
        public bool IsGreenArea {  get; set; }
    }
}
