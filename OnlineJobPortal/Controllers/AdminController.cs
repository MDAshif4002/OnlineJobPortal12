using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using OnlineJobPortal.Models;

namespace OnlineJobPortal.Controllers
{
    public class AdminController : Controller
    {
        public AppDbContext _context;
        public IWebHostEnvironment _environment;
        public AdminController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return RedirectToAction("Index", "Website");
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Website");
        }

        public IActionResult Slider()
        {

            if (HttpContext.Session.GetString("admin") == null)
            {
                return RedirectToAction("Index", "Website");
            }

            var data = _context.slider.ToList();
            return View(data);

        }

        [HttpPost]
        public async Task<IActionResult> AddSlider(IFormFile image)
        {
            string folderpath = Path.Combine(_environment.WebRootPath, "slider");
            string filename = image.FileName;
            string filepath = Path.Combine(folderpath, filename);
            var stream = new FileStream(filepath, FileMode.Create);
            await image.CopyToAsync(stream);

            slider s = new slider();
            s.image = filename;

            _context.slider.Add(s);
            _context.SaveChanges();

            return RedirectToAction("Slider");
        }

        public IActionResult DeleteSlider(int id)
        {
            var data = _context.slider.Find(id);
            string filename = data.image;
            if (filename != null)
            {
                string folderpath = Path.Combine(_environment.WebRootPath, "slider");
                string filepath = Path.Combine(folderpath, filename);
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
            }

            _context.slider.Remove(data);
            _context.SaveChanges();
            return RedirectToAction("Slider");

        }

        public IActionResult Users()
        {
            var data = _context.register.Where(x => x.deletestatus == null || x.deletestatus==false).ToList();
            return View(data);
        }

        public IActionResult DeletedUsers()
        {
            var data = _context.register.Where(x => x.deletestatus == true).ToList(); 
            return View(data);
        }

        public IActionResult RestoreUser(int id)
        {
            var data = _context.register.Find(id);
            data.deletestatus = false; 
            _context.register.Update(data);
            _context.SaveChanges();
            return RedirectToAction("DeletedUsers");
        }

        public IActionResult UserSoftDelete(int id)
        {
            var data = _context.register.Find(id);
            data.deletestatus = true;
            _context.register.Update(data);
            _context.SaveChanges();
            TempData["alert"] = "User Deleted Successfully";
            return RedirectToAction("Users");
        }


        public IActionResult Category()
        {
            var data = _context.category.ToList();
            return View(data);
        }

        public IActionResult CategoryStatus(int id)
        {
            var data = _context.category.Find(id);
            if (data.visiblestatus == null || data.visiblestatus == false)
            {
                data.visiblestatus = true;
            }
            else
            {
                data.visiblestatus = false;
            }

            _context.category.Update(data);
            _context.SaveChanges();
            return RedirectToAction("Category");
        }

        [HttpPost]
        public async Task<IActionResult> SaveCategory(string title, string description, IFormFile picture)  
        {
            string folderpath = Path.Combine(_environment.WebRootPath, "category");
            string filename = picture.FileName;
            string filepath = Path.Combine(folderpath, filename);
            var stream = new FileStream(filepath, FileMode.Create);
            await picture.CopyToAsync(stream);

            category c = new category();
            c.title = title;
            c.description = description;
            c.picture = filename;

            _context.category.Add(c);
            _context.SaveChanges();
            return RedirectToAction("Category");

        }

        public IActionResult Product()
        {
            ViewBag.categories = _context.category.ToList();

            var data = _context.product.ToList();

            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> SaveProduct(string name, string price, string description, IFormFile image, string cid)
        {
            string folderpath = Path.Combine(_environment.WebRootPath, "product");
            string filename = image.FileName;
            string filepath = Path.Combine(folderpath, filename);
            var stream = new FileStream(filepath, FileMode.Create);
            await image.CopyToAsync(stream);

            product p = new product();
            p.name = name;
            p.price = price;    
            p.description = description;
            p.image = filename;
            p.cid = int.Parse(cid);

            _context.product.Add(p);
            _context.SaveChanges();

            return RedirectToAction("Product");
        }

        public IActionResult SingleOrders()
        {
            var data = _context.singleorder.ToList();
            return View(data);
        }
        public IActionResult InvoiceView(int id)
        {
            var data = _context.singleorder.Find(id);
            return View(data);
        }
        public IActionResult CartOrders()
        {
            var data = _context.checkout.ToList();
            return View(data);
        }
        public IActionResult CartInvoice(int id)
        {
            var checkoutdata = _context.checkout.Find(id);
            var cartdata = _context.cart.Where(x => x.orderid == id.ToString()).ToList();

            List<cartpage> cartpage = new List<cartpage>();
            double totalamount = 0;
            foreach(var data in cartdata)
            {
                cartpage cp = new cartpage();
                var pdata = _context.product.Find(int.Parse(data.productid));

                cp.productname = pdata.name;
                cp.productimage = pdata.image;
                cp.productid = pdata.id.ToString();

                cp.qty = data.qty;
                cp.price = data.price;
                cp.subtotal = (Convert.ToInt16(data.qty) * Convert.ToInt16(data.price)).ToString();
                cp.cartid = data.id.ToString();

                totalamount = totalamount + double.Parse(cp.subtotal);


                cartpage.Add(cp);

            }

            cartinvoice cartinvoice = new cartinvoice
            {
                checkout = checkoutdata,
                cartpage = cartpage
            };
            ViewBag.totalamount = totalamount;
            return View(cartinvoice);
        }
    }
}