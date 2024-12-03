using System.ComponentModel.DataAnnotations;

namespace OnlineJobPortal.Models
{
    public class register
    {
        [Key]
        public int id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? mobile { get; set; }
        public string? address { get; set; }

        public bool? deletestatus { get; set; } 
        public string? otp { get; set; }

    }
}
