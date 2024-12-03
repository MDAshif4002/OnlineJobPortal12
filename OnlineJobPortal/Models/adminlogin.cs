using System.ComponentModel.DataAnnotations;

namespace OnlineJobPortal.Models
{
    public class adminlogin
    {
        [Key]
        public int id { get; set; }
        public string? email { get; set; }   
        public string? password { get; set; }

    }
}
