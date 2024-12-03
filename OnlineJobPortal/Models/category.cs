using System.ComponentModel.DataAnnotations;

namespace OnlineJobPortal.Models
{
    public class category
    {
        [Key]
        public int id { get; set; }
        public string? title { get; set; }   
        public string? description { get; set; }
        public string? picture { get; set; }
        public bool? visiblestatus { get; set; }

    }
}
