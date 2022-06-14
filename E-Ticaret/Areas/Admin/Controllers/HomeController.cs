using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using E_Ticaret.Areas.Admin.Models;

namespace E_Ticaret.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly UserContext _context;

        public HomeController(UserContext context)
        {
            _context = context;
        }

        // GET: Admin/Home
        public IActionResult Index()
        {
             return View();
                         
        }
        public IActionResult Login([Bind("UserEmail,UserPassword")] User user) //login işlemini yapıyoruz.
        {
            var dbUser = _context.Users.FirstOrDefault(m => m.UserEmail == user.UserEmail);
            SHA256 sHA256;
            byte[] passwordBytes, hashedBytes;
            string passwordControl;
            if (dbUser != null)
            {
                //şifrenin şifre olarak kaydolmasını sağlıyor.
                sHA256 = SHA256.Create();
                passwordBytes = Encoding.ASCII.GetBytes(user.UserEmail.Trim() + user.UserPassword.Trim()); 
                hashedBytes = sHA256.ComputeHash(passwordBytes);
                passwordControl = BitConverter.ToString(hashedBytes).Replace("-", "");
                if(passwordControl == dbUser.UserPassword)
                {
                    //session'da guest adında değişken tanımladı. dbuser içinde userId'yi tutuyor.
                    //Örneğin guest=1(1 userid oluyor)
                    this.HttpContext.Session.SetString("guest",dbUser.UserId.ToString());
                    //gerekli yetkileri veriyoruz admine.
                    this.HttpContext.Session.SetString("ViewUsers",dbUser.ViewUsers.ToString());
                    this.HttpContext.Session.SetString("CreateUser",dbUser.CreatUser.ToString());
                    this.HttpContext.Session.SetString("EditUser",dbUser.CreatUser.ToString());
                    this.HttpContext.Session.SetString("DeleteUser",dbUser.DeleteUser.ToString());

                    this.HttpContext.Session.SetString("ViewSellers", dbUser.ViewSellers.ToString());
                    this.HttpContext.Session.SetString("CreateSeller", dbUser.CreateSeller.ToString());
                    this.HttpContext.Session.SetString("EditSeller", dbUser.EditSeller.ToString());
                    this.HttpContext.Session.SetString("DeleteSeller", dbUser.DeleteSeller.ToString());

                    this.HttpContext.Session.SetString("ViewCategories", dbUser.ViewCategories.ToString());
                    this.HttpContext.Session.SetString("CreateCategory", dbUser.CreateCategory.ToString());
                    this.HttpContext.Session.SetString("EditCategory", dbUser.EditCategory.ToString());
                    this.HttpContext.Session.SetString("DeleteCategory", dbUser.DeleteCategory.ToString());

                    this.HttpContext.Session.SetString("DeleteProduct", dbUser.DeleteProduct.ToString());
                    this.HttpContext.Session.SetString("EditProduct", dbUser.EditProduct.ToString());
                    // add al authorizations to the session.
                    //...
                    //...
                    return RedirectToAction("Index","Users");
                    //Response.Redirect("~/Admin/Users"); //üst satırdaki sayfaya gitmenin muadil yolu.
                }
                //user.UserPassword = BitConverter.ToString(hashedBytes).Replace("-", "");
            }
            return RedirectToAction("Index");
        }

        
    }
}
