using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Ticaret.Models;
using System.Security.Cryptography;
using System.Text;
namespace E_Ticaret.Controllers
{
    public class CustomersController : Controller
    {
        private readonly eTicaretContext _context;

        public CustomersController(eTicaretContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return _context.Customers != null ?
                        View(await _context.Customers.ToListAsync()) :
                        Problem("Entity set 'eTicaretContext.Customers'  is null.");
        }
        public IActionResult Login(string currentUrl)
        {
            ViewData["currentUrl"] = currentUrl;
            return View();
        }
        
        public void ProcessLogin([Bind("CustomerEmail", "CustomerPassword")]Models.Customer customer, string currentUrl)
        {
            

            SHA256 sHA256;
            byte[] passwordBytes, hashedBytes;
            string customerPassword;
            var dbUser = _context.Customers.FirstOrDefault(m => m.CustomerEmail == customer.CustomerEmail);
            if (dbUser != null)
            {
                //şifrenin şifre olarak kaydolmasını sağlıyor. Yani şifreleme işlemi yaptık.
                sHA256 = SHA256.Create();
                passwordBytes = Encoding.ASCII.GetBytes(customer.CustomerEmail.Trim() + customer.CustomerPassword.Trim());
                hashedBytes = sHA256.ComputeHash(passwordBytes);
                customerPassword = BitConverter.ToString(hashedBytes).Replace("-", "");
                if (customerPassword == dbUser.CustomerPassword)
                {
                    //session'da customer adında değişken tanımladı. dbUser içinde customerId'yi tutuyor.
                    //Örneğin customer=1(1 userid oluyor)
                    this.HttpContext.Session.SetString("customer", dbUser.CustomerId.ToString());
                    TransferCard(dbUser.CustomerId, _context, this.HttpContext);
                    Response.Redirect(currentUrl);
                    return;
                }
            }
            Response.Redirect("Login");
        }
        public void TransferCard(long customerId, Models.eTicaretContext eTicaretContext, HttpContext httpContext, string newCart = null)
        {
            //    //kişi login olduğu an da sepetinin veritabanına aktarılması.


            Models.Product product;
            string[] cartItems;
            string[] itemDetails;
            OrderDetail orderDetail;
            string cartItem;
            CookieOptions cookieOptions = new CookieOptions();

            string cart;
            if (newCart == null)
            {
                cart = Request.Cookies["cart"];
            }
            else
            {
                cart = newCart;
            }
            if (cart == "")
            {
                cart = null;
            }
            short productId;

            Order order;
            if (httpContext.Session.GetString("order") == null)
            {
                order = new Order();
                order.AllDelivered = false;
                order.IsCart = true;
                order.Cancelled = false;
                order.CustomerId = customerId;
                order.PaymentComplete = false;
                order.TimeStamp = DateTime.Now;
            }
            else
            {
                order = eTicaretContext.Orders.FirstOrDefault(o => o.OrderId == Convert.ToInt64(httpContext.Session.GetString("order")));
            }
            order.OrderDetails = new List<OrderDetail>();
            order.OrderPrice = 0;

            if (cart != null)
            {
                cartItems = cart.Split(',');
                for (short i = 0; i < cartItems.Length; i++)
                {

                    orderDetail = new OrderDetail();
                    cartItem = cartItems[i];
                    itemDetails = cartItem.Split(':');
                    productId = Convert.ToInt16(itemDetails[0]);
                    product = eTicaretContext.Products.FirstOrDefault(m => m.ProductId == productId);
                    orderDetail.Product = product;
                    orderDetail.Count = Convert.ToByte(itemDetails[1]);
                    orderDetail.Price = product.ProductPrice * orderDetail.Count;
                    order.OrderDetails.Add(orderDetail);

                }
                if (httpContext.Session.GetString("order") == null)
                {
                    eTicaretContext.Add(order);
                    eTicaretContext.SaveChanges();
                    httpContext.Session.SetString("order", order.OrderId.ToString());
                }
                else
                {
                    eTicaretContext.Update(order);
                    eTicaretContext.SaveChanges();
                }

            }
            else
            {
                if (httpContext.Session.GetString("order") == null)
                {
                    eTicaretContext.Remove(order);
                    eTicaretContext.SaveChanges();
                    httpContext.Session.Remove("order");
                }
            }
        }
        // GET: Customers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create(string currentUrl, bool noPassword = false)
        {
            ViewData["noPassword"] = noPassword;
            ViewData["currentUrl"] = currentUrl;
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerName,CustomerSurname,CustomerEmail,CustomerPhone,CustomerPassword,ConfirmPassword,CustomerAddress,IsDeleted")] Customer customer, string currentUrl)
        {
            if (ModelState.IsValid)
            {

                SHA256 sHA256;
                byte[] passwordBytes, hashedBytes;
                string customerPassword;

                //şifrenin şifre olarak kaydolmasını sağlıyor. Yani şifreleme işlemi yaptık.
                sHA256 = SHA256.Create();
                passwordBytes = Encoding.ASCII.GetBytes(customer.CustomerEmail.Trim() + customer.CustomerPassword.Trim());
                hashedBytes = sHA256.ComputeHash(passwordBytes);
                customerPassword = BitConverter.ToString(hashedBytes).Replace("-", "");
                customer.CustomerPassword = customerPassword;


                _context.Add(customer);
                await _context.SaveChangesAsync();
                this.HttpContext.Session.SetString("customer", customer.CustomerId.ToString());
                TransferCard(customer.CustomerId, _context, this.HttpContext);
                return Redirect(currentUrl);

            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("CustomerId,CustomerName,CustomerSurname,CustomerEmail,CustomerPhone,CustomerPassword,CustomerAddress,IsDeleted")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'eTicaretContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(long id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
