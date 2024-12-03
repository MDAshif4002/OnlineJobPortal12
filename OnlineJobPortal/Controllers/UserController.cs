using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnlineJobPortal.Models;
using OnlineJobPortal.Services;

namespace OnlineJobPortal.Controllers
{
    public class UserController : Controller
    {
        public AppDbContext _context;
        
        public UserController(AppDbContext context)
        {
            _context = context;
            
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                TempData["loginstatus"] = "true";
            }
            base.OnActionExecuting(context);
        }

        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("user") == null)
            {
                return RedirectToAction("Login", "Website");
            }
            return View();
        }
        public IActionResult MyOrders()
        {
            if (HttpContext.Session.GetString("user") == null)
            {
                return RedirectToAction("Login", "Website");
            }

            string userid = HttpContext.Session.GetString("userid");

            var data = _context.cart.Where(x => x.userid == userid && x.orderid == "0").ToList();

            List<cartpage> cartdata = new List<cartpage>();
            double totalAmount = 0;
            foreach (var item in data)
            {
                var prodata = _context.product.Find(int.Parse(item.productid));

                cartpage cart = new cartpage();
                cart.productname = prodata.name;
                cart.productimage = prodata.image;
                cart.productid = prodata.id.ToString();

                cart.qty = item.qty;
                cart.price = item.price;
                cart.subtotal = (Convert.ToInt16(item.price) * Convert.ToInt16(item.qty)).ToString();
                cart.cartid = item.id.ToString();

                totalAmount = totalAmount + double.Parse(cart.subtotal);
                cartdata.Add(cart);
            }
            ViewBag.totalamount = totalAmount;

            return View(cartdata);
        }
    }
}