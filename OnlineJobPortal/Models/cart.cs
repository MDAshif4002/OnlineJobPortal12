using System.ComponentModel.DataAnnotations;

namespace OnlineJobPortal.Models
{
    public class cart
    {
        [Key]
        public int id { get; set; }
        public string? productid { get; set; }
        public string? userid { get; set; }
        public string? qty { get; set; }
        public string? orderid { get; set; }
        public string? price { get; set; }
    }
}
