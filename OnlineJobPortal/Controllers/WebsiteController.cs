using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnlineJobPortal.Models;
using OnlineJobPortal.Services;
using System.Diagnostics.Eventing.Reader;

namespace OnlineJobPortal.Controllers
{
    public class WebsiteController : Controller
    {
        public AppDbContext _context;
        public EmailSender _emailSender;
        public WebsiteController(AppDbContext context, EmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(HttpContext.Session.GetString("user") != null)
            {
                TempData["loginstatus"] = "true";
            }
            base.OnActionExecuting(context);
        }

        public IActionResult Index()
        {
            var data = _context.slider.ToList();
            var data1 = _context.category.Where(x=>x.visiblestatus==true).ToList();

            var alldata = new HomePage
            {
                category = data1,
                slider = data
            };


            return View(alldata);
        }

        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(string email, string password)
        {
            var data = _context.adminlogin.FirstOrDefault(x => x.email == email && x.password == password);
            if (data != null)
            {
                HttpContext.Session.SetString("admin", email);
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                TempData["msg"] = "Email or Password is incorrect";
                return RedirectToAction("AdminLogin");
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveRegister(register r, string email)
        {
            var data = _context.register.FirstOrDefault(x => x.email == r.email );   

            if(data != null)
            {
                _context.register.Add(r);
                _context.SaveChanges();

                TempData["alert"] = "registration_success";
                TempData["username"] = r.name;

                //otp strart
                
                Random rnd = new Random();
                int num = rnd.Next(10000, 99999);

                HttpContext.Session.SetString("otp", num.ToString());
                HttpContext.Session.SetString("email", email);

                string sendto = email;
                string subject = "OTP for Registration";
                string mail = "Dear User, Your OTP for Registration is-" + num;
                await _emailSender.SendEmailAsync(sendto, subject, mail);

                return RedirectToAction("Otp");
                //otp end
            }
            else
            {
                TempData["error"] = "Email or Mobile Already Registered";
                return RedirectToAction("Register");
            }


            //return RedirectToAction("Register");
        }

        public IActionResult Otp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Otp(string otp)
        {
            string originalotp = HttpContext.Session.GetString("otp");
            if(otp==originalotp)
            {
                string email = HttpContext.Session.GetString("email");
                var data = _context.register.FirstOrDefault(x => x.email == email);
                data.otp = otp;
                _context.register.Update(data);
                _context.SaveChanges();
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }
            else
            {
                TempData["msg"] = "You Entered Incorrect OTP";
                return RedirectToAction("Otp");
            }
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var data = _context.register.FirstOrDefault(x => x.email == email && x.password == password && x.deletestatus != true);
            if(data != null)
            {
                HttpContext.Session.SetString("user", email);
                HttpContext.Session.SetString("userid", data.id.ToString());

                TempData["loginstatus"] = "true";
            
                return RedirectToAction("Index", "User");
            }
            else
            {
                TempData["msg"] = "Email or Password incorrect";
                return RedirectToAction("Login");
            }
        }
        public IActionResult Products(int id)
        {
            var data = _context.product.Where(x => x.cid == id).ToList();
            ViewBag.categories = _context.category.Where(x=> x.visiblestatus == true).ToList();    
            return View(data);
        }

        public IActionResult ProductDetails(int id)
        {
            var data = _context.product.Find(id);

            ViewBag.categories = _context.category.Where(x => x.visiblestatus == true).ToList();

            return View(data);
        }

        public IActionResult BuyNow(int id)
        {
            var data = _context.product.Find(id);
            return View(data);  
        }

        [HttpPost]
        public IActionResult OrderNow(IFormCollection form)
        {
            singleorder order = new singleorder();
            order.name = form["name"];
            order.mobile = form["mobile"];
            order.email = form["email"];
            order.pincode = form["pincode"];
            order.address = form["address"];
            order.productid = form["id"];

            var data = _context.product.Find( int.Parse( form["id"].ToString() ) );
            // var data = _context.product.find( form["id"] );

            order.productname = data.name;
            order.productprice = data.price;
            order.mode = form["paymentmode"];

            order.paymentstatus = "pending";
            order.transactionid = "0";

            _context.singleorder.Add(order);
            _context.SaveChanges();

            if (form["paymentmode"]=="cod")
            {
                return RedirectToAction("OrderPlaced");
            }
            else
            {
                TempData["name"] = order.name;
                TempData["email"] = order.email;
                TempData["mobile"] = order.mobile;
                TempData["amount"] = data.price;
                TempData["orderid"] = order.id;

                return RedirectToAction("PayNow");

            }


        }

        public IActionResult OrderPlaced()
        {
            return View();
        }

        public IActionResult PayNow()
        {
            return View();
        }

        public IActionResult PaymentSuccess()
        {
            string txnid = Request.Query["txnid"];
            string orderid = Request.Query["orderid"];

            var data = _context.singleorder.Find(int.Parse(orderid) );

            data.transactionid = txnid;
            data.paymentstatus = "success";

            _context.singleorder.Update(data);
            _context.SaveChanges();

            return RedirectToAction("OrderPlaced");
        }

        public IActionResult PaymentFailed()
        {
            return View();
        }
        public IActionResult AddToCart(int id)
        {
            if(HttpContext.Session.GetString("user") == null)
            {
                return RedirectToAction("Login","Website");
            }

            var product = _context.product.Find(id);
            string price = product.price;
            string userid = HttpContext.Session.GetString("userid");

            cart c = new cart();
            c.productid = id.ToString();
            c.userid = userid;
            c.qty = "1";
            c.orderid = "0";
            c.price = price;

            _context.cart.Add(c);
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }
        public IActionResult Cart()
        {
            if (HttpContext.Session.GetString("user") == null)
            {
                return RedirectToAction("Login", "Website");
            }

            string userid = HttpContext.Session.GetString("userid");

            var data = _context.cart.Where(x => x.userid == userid && x.orderid == "0").ToList();
            
            List<cartpage> cartdata = new List<cartpage>();
            double totalAmount = 0;
            foreach(var item in data)
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
        public IActionResult DeleteCart(int id)
        {
            var data = _context.cart.Find(id);
            _context.cart.Remove(data);
            _context.SaveChanges();
            return RedirectToAction("Cart", "Website");
        }
        public IActionResult CartMinus(int id)
        {
            var data = _context.cart.Find(id);
            int oldqty = int.Parse(data.qty);
            if(oldqty > 1)
            {
                int newqty = oldqty - 1;
                data.qty = newqty.ToString();
                _context.cart.Update(data);
                _context.SaveChanges();
            }
            return RedirectToAction("Cart");
        }
        public IActionResult CartPlus(int id)
        {
            var data = _context.cart.Find(id);
            int oldqty = int.Parse(data.qty);

            int newqty = oldqty + 1;

            data.qty = newqty.ToString();
            _context.cart.Update(data);
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }
        public IActionResult Checkout()
        {
            if(HttpContext.Session.GetString("user") == null )
            {
                return RedirectToAction("Login");
            }

            string userid = HttpContext.Session.GetString("userid");
            var data = _context.register.Find(int.Parse(userid));

            return View(data);
        }
        [HttpPost]
        public IActionResult Checkout(IFormCollection form)
        {
            checkout c = new checkout();
            c.name = form["name"];
            c.mobile = form["mobile"];
            c.email = form["email"];
            c.city = form["city"];
            c.pincode = form["pincode"];
            c.address = form["address"];
            c.mode = form["mode"];
            c.paymentstatus = "pending";
            c.transactionid = "0";

            string userid = HttpContext.Session.GetString("userid");
            c.userid = userid;

            _context.checkout.Add(c);
            _context.SaveChanges();

            double totalamount = 0;
            int orderid = c.id;
            var data = _context.cart.Where(x => x.userid == userid && x.orderid == "0").ToList();
            foreach (var item in data)
            {
                item.orderid = orderid.ToString();

                totalamount += Convert.ToInt16(item.price) * Convert.ToInt16(item.qty);


                _context.cart.Update(item);
            }

            _context.SaveChanges();

            if(c.mode == "cod")
            {
                return RedirectToAction("OrderPlaced");
            }
            else
            {
                TempData["name"] = c.name;
                TempData["mobile"] = c.mobile;
                TempData["email"] = c.email;
                TempData["orderid"] = c.id;
                TempData["amount"] = totalamount.ToString();

                return RedirectToAction("PayNow2");
            }
        }
        public IActionResult PayNow2()
        {
            return View();
        }
        public IActionResult PaymentSuccess2()
        {
            string orderid = Request.Query["orderid"];
            string txnid = Request.Query["txnid"];

            var data = _context.checkout.Find(int.Parse(orderid));

            data.transactionid = txnid;
            data.paymentstatus = "success";

            _context.checkout.Update(data);
            _context.SaveChanges();

            return RedirectToAction("OrderPlaced");
        }
    }
}   