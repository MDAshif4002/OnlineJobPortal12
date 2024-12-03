namespace OnlineJobPortal.Models
{
    public class cartinvoice
    {
        public checkout checkout { get; set; }

        public List<cartpage> cartpage { get; set; }
        //public List<cartpage> cartpage { get; set; }
    }
}
