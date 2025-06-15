using API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class GetPostDto
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
        public User User { get; set; }
        public ICollection<Media> MediaItems { get; set; }

    }
}
