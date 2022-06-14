using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Ticaret.Models;

namespace E_Ticaret.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class ProductsController : Controller
    {
        private readonly eTicaretContext _context;

        public ProductsController(eTicaretContext context)
        {
            _context = context;
        }

        // GET: Seller/Products
        public async Task<IActionResult> Index()
        {
            
            int? merchantId = HttpContext.Session.GetInt32("merchant");
            if (merchantId == null) //merchantId'si yoksa hata ver.
            {
                return RedirectToAction("Index", "Home");
            }
            var eTicaretContext = _context.Products.Include(p => p.Brand).Include(p => p.Category).Include(p => p.Seller).Where(p=>p.SellerId ==merchantId.Value);
            return View(await eTicaretContext.ToListAsync());
        }
        public Product? Product(long id)
        {
            return _context.Products.Include(m => m.Brand).Include(m => m.Seller).FirstOrDefault(m => m.ProductId == id);
        }
        // GET: Seller/Products/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (HttpContext.Session.GetInt32("merchant") == null)
            {
                return RedirectToAction("Index", "Home");
            }
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

        // GET: Seller/Products/Create
        public IActionResult Create()
        {
            int? merchantId = HttpContext.Session.GetInt32("merchant");
            if (HttpContext.Session.GetInt32("merchant") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["SellerId"] = merchantId.Value;
            return View();
        }

        // POST: Seller/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductPrice,Image,Description,ProductRate,CategoryId,IsDeleted,BrandId,SellerId")] Product product)
        {
            FileStream imageStream;
            string fileName;

            int? merchantId = HttpContext.Session.GetInt32("merchant");
            if (HttpContext.Session.GetInt32("merchant") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                if(product.Image != null)
                {
                    for (short i = 0; i < product.Image.Length; i++)
                    {
                        fileName = Directory.GetCurrentDirectory() + "\\wwwroot\\Image\\" + product.ProductId.ToString() + "-" + i.ToString() + Path.GetExtension(product.Image[i].FileName);
                        imageStream = new FileStream(fileName, FileMode.Create);
                        await product.Image[i].CopyToAsync(imageStream);
                        imageStream.Close();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SellerId"] = merchantId.Value;
            return View(product);
        }

        // GET: Seller/Products/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (HttpContext.Session.GetInt32("merchant") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SellerId"] = new SelectList(_context.Sellers, "SellerId", "Phone", product.SellerId);
            return View(product);
        }

        // POST: Seller/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ProductId,ProductName,ProductPrice,Description,ProductRate,CategoryId,IsDeleted,BrandId,SellerId")] Product product)
        {
            FileStream imageStream;
            string fileName;
            if (HttpContext.Session.GetInt32("merchant") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    for (byte i = 0; i < Request.Form.Files.Count; i++)
                    {
                        //sunucuya gidecek kodu yazacağız.

                        var file = Request.Form.Files[i];
                        var fileNumber = file.Name.Substring(5);
                        fileName = Directory.GetCurrentDirectory() + "\\wwwroot\\Image\\" + product.ProductId.ToString() + "-" + fileNumber + Path.GetExtension(file.FileName);
                        imageStream = new FileStream(fileName, FileMode.OpenOrCreate);
                        await file.CopyToAsync(imageStream);
                        imageStream.Close();


                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SellerId"] = new SelectList(_context.Sellers, "SellerId", "Phone", product.SellerId);
            return View(product);
        }

        // GET: Seller/Products/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (HttpContext.Session.GetInt32("merchant") == null)
            {
                return RedirectToAction("Index", "Home");
            }
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

        // POST: Seller/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (HttpContext.Session.GetInt32("merchant") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (_context.Products == null)
            {
                return Problem("Entity set 'eTicaretContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(long id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
