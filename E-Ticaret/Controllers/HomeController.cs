using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Ticaret.Models;

namespace E_Ticaret.Controllers
{
    public class HomeController : Controller
    {
        private readonly eTicaretContext _context;

        public HomeController(eTicaretContext context)
        {
            _context = context;
        }
        // GET: Home
        public async Task<IActionResult> Index()
        {
            var eticaretContext = _context.Products.Include(p => p.Brand).Include(p => p.Category).Include(p => p.Seller).Where(p => p.IsDeleted == false);
            return View(await eticaretContext.ToListAsync());
        }
        public async Task<IActionResult> JsonIndex()
        {
            //var eticaretContext = _context.Products.Include(p => p.Brand).Include(p => p.Category).Include(p => p.Seller).Where(p => p.IsDeleted == false);
            return Json(await _context.Products.ToListAsync());
        }
        // GET: Home/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
