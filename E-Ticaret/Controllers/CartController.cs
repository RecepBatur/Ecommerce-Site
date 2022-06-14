using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace E_Ticaret.Controllers
{
    public class CartController : Controller
    {
        public struct CartProduct
        {
            public Models.Product Product;
            public short Count;
            public float Total;
        }
        public string Add(short id)
        {
            CookieOptions cookieOptions = new CookieOptions();
            string? cart = Request.Cookies["cart"];
            string[] cartItems;
            string[] itemDetails;
            short itemCount;
            string newCart = "";
            string cartItem;
            short totalCount = 0;
            bool itemExist = false;
            if (cart == null)
            {
                newCart = id.ToString() + ":1";
                totalCount = 1;
            }
            else
            {
                cartItems = cart.Split(',');// 9:4,37:1,56:2,18:9
                for (short i = 0; i < cartItems.Length; i++)
                {
                    cartItem = cartItems[i]; // 9:4
                    itemDetails = cartItem.Split(':');//9[0] 4[1]
                    itemCount = Convert.ToInt16(itemDetails[1]);//4
                    if (itemDetails[0] == id.ToString())
                    {
                        itemCount++;
                        itemExist = true;
                    }
                    totalCount += itemCount; // totalc = totalc + itemc
                    newCart = newCart + itemDetails[0] + ":" + itemCount.ToString();
                    if (i < cartItems.Length - 1)
                    {
                        newCart = newCart + ",";
                    }

                }
                if (itemExist == false)
                {
                    newCart = newCart + "," + id.ToString() + ":1";
                    totalCount++;
                }
            }
            cookieOptions.Path = "/";
            cookieOptions.Expires = DateTime.MaxValue;
            Response.Cookies.Append("cart", newCart, cookieOptions);
            return totalCount.ToString();
        }
        public IActionResult Index()
        {
            DbContextOptions<Models.eTicaretContext> options = new DbContextOptions<Models.eTicaretContext>();
            Models.eTicaretContext eTicaretContext = new Models.eTicaretContext(options);
            Areas.Seller.Controllers.ProductsController productsController = new Areas.Seller.Controllers.ProductsController(eTicaretContext);
            Models.Product product;

            CookieOptions cookieOptions = new CookieOptions();
            short productId;
            byte i = 0;
            string? cart = Request.Cookies["cart"];
            string[] cartItems;
            string[] itemDetails;
            short itemCount;
            string cartItem;
            List<CartProduct> cartProducts = new List<CartProduct>();
            float cartTotal = 0;
            if (cart == null)
            {
                ViewBag.message = "Sepetinizde ürün bulunmamaktadır.";
                return View();
            }
            else
            {
                cartItems = cart.Split(',');// 9:4,37:1,56:2,18:9
                for (i = 0; i < cartItems.Length; i++)
                {
                    cartItem = cartItems[i]; // 9:4
                    itemDetails = cartItem.Split(':');//9[0] 4[1]
                    CartProduct cartProduct = new CartProduct();
                    productId = Convert.ToInt16(itemDetails[0]);
                    product = productsController.Product(productId);
                    cartProduct.Product = product;
                    cartProduct.Count = Convert.ToInt16(itemDetails[1]);
                    cartProduct.Total = cartProduct.Count * product.ProductPrice;
                    cartProducts.Add(cartProduct);
                    cartTotal += cartProduct.Total;


                }
                ViewData["product"] = cartProducts;
                ViewData["cartTotal"] = cartTotal;
            }
            cookieOptions.Path = "/";
            return View();
        }
        public string CalculateTotal(long id, byte count)
        {
            DbContextOptions<Models.eTicaretContext> options = new DbContextOptions<Models.eTicaretContext>();
            Models.eTicaretContext eTicaretContext = new Models.eTicaretContext(options);
            Areas.Seller.Controllers.ProductsController productsController = new Areas.Seller.Controllers.ProductsController(eTicaretContext);
            Models.Product product = productsController.Product(id);

            float subTotal = 0;
            float productPrice;
            subTotal = product.ProductPrice * count;
            changeCookie(id, count);
            return subTotal.ToString();
        }
        private void changeCookie(long id, byte count)
        {
            DbContextOptions<Models.eTicaretContext> options = new DbContextOptions<Models.eTicaretContext>();
            Models.eTicaretContext eTicaretContext = new Models.eTicaretContext(options);
            CustomersController customersController = new Controllers.CustomersController(eTicaretContext);

            CookieOptions cookieOptions = new CookieOptions();
            string? cart = Request.Cookies["cart"];
            string[] cartItems;
            string[] itemDetails;
            short itemCount;
            string newCart = "";
            string cartItem;
            short totalCount = 0;
            bool itemExist = false;

            cartItems = cart.Split(',');// 9:4,37:1,56:2,18:9
            for (short i = 0; i < cartItems.Length; i++)
            {
                cartItem = cartItems[i]; // 9:4
                itemDetails = cartItem.Split(':');//9[0] 4[1]
                itemCount = Convert.ToInt16(itemDetails[1]);//4
                if (itemDetails[0] == id.ToString())
                {
                    itemCount = count;

                }
                totalCount += itemCount; // totalc = totalc + itemc
                newCart = newCart + itemDetails[0] + ":" + itemCount.ToString();

                if (i < cartItems.Length - 1)
                {
                    newCart = newCart + ",";
                }

            }
            if (newCart.Substring(newCart.Length - 1) == ",")
            {
                newCart.Remove(newCart.Length - 1);
            }
            cookieOptions.Path = "/";
            cookieOptions.Expires = DateTime.MaxValue;
            Response.Cookies.Append("cart", newCart, cookieOptions);
            if (this.HttpContext.Session.GetString("customer") != null)
            {
                customersController.TransferCard(Convert.ToInt64(this.HttpContext.Session.GetString("customer")), eTicaretContext, this.HttpContext, "");
            }
            ViewData["totalCount"] = totalCount;
        }
        public void EmptyBasket()
		{
            DbContextOptions<Models.eTicaretContext> options = new DbContextOptions<Models.eTicaretContext>();
            Models.eTicaretContext eTicaretContext = new Models.eTicaretContext(options);
            CustomersController customersController = new Controllers.CustomersController(eTicaretContext);
            
            Response.Cookies.Delete("cart");
            if(this.HttpContext.Session.GetString("customer") != null)
            {
                customersController.TransferCard(Convert.ToInt64(this.HttpContext.Session.GetString("customer")), eTicaretContext, this.HttpContext, "");
            }
            Response.Redirect("Index");
		}

    }

}