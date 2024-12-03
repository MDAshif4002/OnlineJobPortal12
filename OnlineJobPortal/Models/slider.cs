using System.ComponentModel.DataAnnotations;

namespace OnlineJobPortal.Models
{
    public class slider
    {
        [Key]
        public int id {  get; set; }
        public string? image { get; set; }
    }
}
