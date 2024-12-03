using System.ComponentModel.DataAnnotations;

namespace OnlineJobPortal.Models
{
    public class checkout
    {
        [Key]
        public int id { get; set; }
        public string? name { get; set; }
        public string? mobile { get; set; }
        public string? email { get; set; }
        public string? city { get; set; }
        public string? pincode { get; set; }
        public string? address { get; set; }
        public string? mode { get; set; }
        public string? paymentstatus { get; set; }
        public string? transactionid { get; set; }
        public string? userid { get; set; }
    }
}
