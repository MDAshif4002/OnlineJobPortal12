using System.ComponentModel.DataAnnotations;
using System.Security;

namespace OnlineJobPortal.Models
{
    public class product
    {
        [Key]
        public int id {  get; set; }
        public string? name { get; set; }
        public string? price { get; set; }
        public string? description { get; set; }
        public string? image {  get; set; }
        public int? cid { get; set; }


    }
}
