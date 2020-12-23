using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sky.Areas.Identity.Data;
using Sky.Data;
using Sky.Models;
using static Sky.Data.Helper;

namespace Sky.Controllers
{
    public class ProductController : Controller
    {
        const int USER_PER_PAGE = 16;
        private readonly SkyAppDbContext _context;
        private readonly UserManager<SkyUser> _userManager;
        private readonly SignInManager<SkyUser> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(SkyAppDbContext context,
            UserManager<SkyUser> userManager,
            SignInManager<SkyUser> signInManager,
            IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment;
        }

        
        [NoDirectAccess]
        public async Task<IActionResult> AddtoCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                if (!_signInManager.IsSignedIn(User))
                {
                    ViewData["Mess"] = "Bạn cần đăng nhập để tiếp tục";

                    return View();
                }

                var iduser = _userManager.GetUserId(User);

                var cart = _context.CartDbSet.Where(c => c.UserId == iduser).ToList().FirstOrDefault();

                var check = _context.CartDetailDbSet.Where(c => c.CartId == cart.CartId && c.ProductId == id).FirstOrDefault();

                if (check != null)
                {
                    check.ProductQuantity += 1;

                    ViewData["Mess"] = "Đã cập nhật +1 !";
                }
                else
                {
                    var cartde = new CartDetail { CartId = cart.CartId, ProductId = (int)id, ProductQuantity = 1 };

                    _context.CartDetailDbSet.Add(cartde);

                    ViewData["Mess"] = "Thêm thành công";
                }

                await _context.SaveChangesAsync();


            }
            catch
            {
                ViewData["Mess"] = "Thất bại";
            }
            return View();
        }


        
        [NoDirectAccess]
        public async Task<IActionResult> AddtoCart2(string id, string quantity)
        {
            if (id == null || quantity == null)
            {
                return NotFound();
            }

            try
            {
                if (!_signInManager.IsSignedIn(User))
                {
                    ViewData["Mess"] = "Bạn cần đăng nhập để tiếp tục";

                    return View();
                }

                var iduser = _userManager.GetUserId(User);

                var cart = _context.CartDbSet.Where(c => c.UserId == iduser).ToList().FirstOrDefault();

                var check = _context.CartDetailDbSet.Where(c => c.CartId == cart.CartId && c.ProductId == Convert.ToInt32(id)).FirstOrDefault();

                if (check != null)
                {
                    check.ProductQuantity += Convert.ToInt32(quantity);

                    ViewData["Mess"] = "Đã cập nhật +" + quantity;

                    await _context.SaveChangesAsync();

                    return View();
                }
                else
                {
                    var cartde = new CartDetail { CartId = cart.CartId, ProductId = Convert.ToInt32(id), ProductQuantity = Convert.ToInt32(quantity) };

                    _context.CartDetailDbSet.Add(cartde);

                    ViewData["Mess"] = "Thành công";
                }
                await _context.SaveChangesAsync();
            }
            catch
            {
                ViewData["Mess"] = "Thất bại";
            }
            return View();
        }

        //-------------------------------------------------------------------------------------------------------------------------
        public async Task<IActionResult> SearchAsync(string q, string sortOrder, int PageNumber)
        {
            ViewData["CurrentFilter"] = q;
            ViewData["Sort"] = sortOrder;

            var skyAppDbContext = _context.ProductDbSet.Where(c => c.ProductName.Contains(q) || c.ProductDescription.Contains(q));

            

            if (!skyAppDbContext.Any())
            {
                ViewBag.Message = "Không tìm thấy từ khóa '" + q + "'";
            }


            switch (sortOrder)
            {
                case "price":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ProductPrice);
                    break;
                case "price_asc":
                    skyAppDbContext = skyAppDbContext.OrderBy(s => s.ProductPrice);
                    break;
                case "view":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ViewCount);
                    break;
                case "new":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ProductDate);
                    break;
                default:
                    skyAppDbContext = skyAppDbContext.OrderBy(s => s.Type.TypeName);
                    break;
            }

            if (PageNumber == 0)
            {
                PageNumber = 1;
            }


            ViewData["PageNumber"] = PageNumber;

            int totalProducts = await skyAppDbContext.CountAsync();

            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalProducts / USER_PER_PAGE);

            skyAppDbContext = skyAppDbContext.Include(p => p.Category).Include(p => p.Type);

            return View(await skyAppDbContext.Skip(USER_PER_PAGE * ((int)ViewData["PageNumber"] - 1)).Take(USER_PER_PAGE).AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> Nam(string searchString, string sortOrder, string type, int PageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["Type"] = type;
            ViewData["Sort"] = sortOrder;

            //Lấy danh sách các loại type
            ViewBag.ListType = _context.ProductDbSet.Where(p => p.Category.CategoryName == "Nam" && p.ProductStatus == true).Select(c => c.Type.TypeName).Distinct().ToList();


            var skyAppDbContext = _context.ProductDbSet.Where(p => p.Category.CategoryName == "Nam" && p.ProductStatus == true);

            if (!String.IsNullOrEmpty(searchString))
            {
                skyAppDbContext = skyAppDbContext.Where(s => s.ProductName.Contains(searchString) || s.ProductDescription.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(type))
            {
                skyAppDbContext = skyAppDbContext.Where(s => s.Type.TypeName.Contains(type));
            }

            if (!skyAppDbContext.Any())
            {
                ViewBag.Message = "Không tìm thấy từ khóa '" + searchString + "'";
            }


            switch (sortOrder)
            {
                case "price":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ProductPrice);
                    break;
                case "price_asc":
                    skyAppDbContext = skyAppDbContext.OrderBy(s => s.ProductPrice);
                    break;
                case "view":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ViewCount);
                    break;
                case "new":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ProductDate);
                    break;
                default:
                    skyAppDbContext = skyAppDbContext.OrderBy(s => s.Type.TypeName);
                    break;
            }

            if (PageNumber == 0)
            {
                PageNumber = 1;
            }
            

            ViewData["PageNumber"] = PageNumber;

            int totalProducts = await skyAppDbContext.CountAsync();

            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalProducts / USER_PER_PAGE);

            skyAppDbContext = skyAppDbContext.Include(p => p.Category).Include(p => p.Type);

            return View(await skyAppDbContext.Skip(USER_PER_PAGE * ((int)ViewData["PageNumber"] - 1)).Take(USER_PER_PAGE).AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> Nu(string searchString, string sortOrder, string type, int PageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["Type"] = type;
            ViewData["Sort"] = sortOrder;

            //Lấy danh sách các loại type
            ViewBag.ListType = _context.ProductDbSet.Where(p => p.Category.CategoryName == "Nu" && p.ProductStatus == true).Select(c => c.Type.TypeName).Distinct().ToList();


            var skyAppDbContext = _context.ProductDbSet.Where(p => p.Category.CategoryName == "Nu" && p.ProductStatus == true);

            if (!String.IsNullOrEmpty(searchString))
            {
                skyAppDbContext = skyAppDbContext.Where(s => s.ProductName.Contains(searchString) || s.ProductDescription.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(type))
            {
                skyAppDbContext = skyAppDbContext.Where(s => s.Type.TypeName.Contains(type));
            }

            if (!skyAppDbContext.Any())
            {
                ViewBag.Message = "Không tìm thấy từ khóa '" + searchString + "'";
            }


            switch (sortOrder)
            {
                case "price":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ProductPrice);
                    break;
                case "price_asc":
                    skyAppDbContext = skyAppDbContext.OrderBy(s => s.ProductPrice);
                    break;
                case "view":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ViewCount);
                    break;
                case "new":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ProductDate);
                    break;
                default:
                    skyAppDbContext = skyAppDbContext.OrderBy(s => s.Type.TypeName);
                    break;
            }

            if (PageNumber == 0)
            {
                PageNumber = 1;
            }


            ViewData["PageNumber"] = PageNumber;

            int totalProducts = await skyAppDbContext.CountAsync();

            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalProducts / USER_PER_PAGE);

            skyAppDbContext = skyAppDbContext.Include(p => p.Category).Include(p => p.Type);

            return View(await skyAppDbContext.Skip(USER_PER_PAGE * ((int)ViewData["PageNumber"] - 1)).Take(USER_PER_PAGE).AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> Category(string cate, string searchString, string sortOrder, string type, int PageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["Type"] = type;
            ViewData["Sort"] = sortOrder;
            ViewData["Cate"] = cate;

            //Lấy danh sách các loại type
            ViewBag.ListType = _context.ProductDbSet.Where(p => p.Category.CategoryName == cate && p.ProductStatus == true).Select(c => c.Type.TypeName).Distinct().ToList();


            var skyAppDbContext = _context.ProductDbSet.Where(p => p.Category.CategoryName == cate && p.ProductStatus == true);

            if (!String.IsNullOrEmpty(searchString))
            {
                skyAppDbContext = skyAppDbContext.Where(s => s.ProductName.Contains(searchString) || s.ProductDescription.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(type))
            {
                skyAppDbContext = skyAppDbContext.Where(s => s.Type.TypeName.Contains(type));
            }

            if (!skyAppDbContext.Any())
            {
                ViewBag.Message = "Không tìm thấy từ khóa '" + searchString + "'";
            }


            switch (sortOrder)
            {
                case "price":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ProductPrice);
                    break;
                case "price_asc":
                    skyAppDbContext = skyAppDbContext.OrderBy(s => s.ProductPrice);
                    break;
                case "view":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ViewCount);
                    break;
                case "new":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ProductDate);
                    break;
                default:
                    skyAppDbContext = skyAppDbContext.OrderBy(s => s.Type.TypeName);
                    break;
            }

            if (PageNumber == 0)
            {
                PageNumber = 1;
            }


            ViewData["PageNumber"] = PageNumber;

            int totalProducts = await skyAppDbContext.CountAsync();

            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalProducts / USER_PER_PAGE);

            skyAppDbContext = skyAppDbContext.Include(p => p.Category).Include(p => p.Type);

            return View(await skyAppDbContext.Skip(USER_PER_PAGE * ((int)ViewData["PageNumber"] - 1)).Take(USER_PER_PAGE).AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> ChiTiet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.ProductDbSet
                .Include(p => p.Category)
                .Include(p => p.Type)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            product.ViewCount += 1;

            _context.Update(product);
            await _context.SaveChangesAsync();

            return View(product);
        }


        //-------------------------------------------------------------------------------------------------------------------------


        // GET: Product
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Index(string searchString, string sortOrder, int PageNumber)
        {
            ViewData["CurrentFilter"] = searchString;

            ViewData["ProductNameSort"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["ProductDateSort"] = sortOrder == "date" ? "date_desc" : "date";

            var skyAppDbContext = from a in _context.ProductDbSet select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                skyAppDbContext = skyAppDbContext.Where(s => s.ProductName.Contains(searchString) || s.ProductDescription.Contains(searchString));

            }

            if (!skyAppDbContext.Any())
            {
                ViewBag.Message = "Không tìm thấy từ khóa '" + searchString + "'";
            }

            switch (sortOrder)
            {
                case "name_desc":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ProductName);
                    break;
                case "ate":
                    skyAppDbContext = skyAppDbContext.OrderBy(s => s.ProductDate);
                    break;
                case "date_desc":
                    skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.ProductDate);
                    break;
                default:
                    skyAppDbContext = skyAppDbContext.OrderBy(s => s.ProductName);
                    break;
            }

            if (PageNumber == 0)
            {
                PageNumber = 1;
            }

            ViewData["PageNumber"] = PageNumber;

            int totalProducts = await skyAppDbContext.CountAsync();

            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalProducts / USER_PER_PAGE);

            skyAppDbContext = skyAppDbContext.Include(p => p.Category).Include(p => p.Type);

            return View(await skyAppDbContext.Skip(USER_PER_PAGE * ((int)ViewData["PageNumber"] - 1)).Take(USER_PER_PAGE).AsNoTracking().ToListAsync());
        }

        // GET: Product/Details/5
        [Authorize(Roles = "Admin, Manager")]
        [NoDirectAccess]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.ProductDbSet
                .Include(p => p.Category)
                .Include(p => p.Type)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        [Authorize(Roles = "Admin, Manager")]
        [NoDirectAccess]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.CategorieDbSet, "CategoryId", "CategoryName");
            ViewData["TypeId"] = new SelectList(_context.TypeDbSet, "TypeId", "TypeName");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Manager")]
        [NoDirectAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductImageFile,ProductDescription,ProductPrice,PreviousPrice,ViewCount,ProductDate,ProductStatus,CategoryId,TypeId")] Product product)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(product.ProductImageFile.FileName);
                string extension = Path.GetExtension(product.ProductImageFile.FileName);
                product.ProductImage = fileName = "SP" + DateTime.Now.ToString("yyMMddHHmmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await product.ProductImageFile.CopyToAsync(fileStream);
                }

                product.ProductDate = DateTime.Now;
                product.ViewCount = 1;

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.CategorieDbSet, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["TypeId"] = new SelectList(_context.TypeDbSet, "TypeId", "TypeName", product.TypeId);
            return View(product);
        }

        // GET: Product/Edit/5
        [Authorize(Roles = "Admin, Manager")]
        [NoDirectAccess]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.ProductDbSet.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.CategorieDbSet, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["TypeId"] = new SelectList(_context.TypeDbSet, "TypeId", "TypeName", product.TypeId);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Manager")]
        [NoDirectAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductImage,ProductImageFile,ProductDescription,ProductPrice,PreviousPrice,ViewCount,ProductDate,ProductStatus,CategoryId,TypeId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    try
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(product.ProductImageFile.FileName);
                        string extension = Path.GetExtension(product.ProductImageFile.FileName);

                        var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", product.ProductImage);
                        if (System.IO.File.Exists(imagePath))
                            System.IO.File.Delete(imagePath);

                        product.ProductImage = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/Image/", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await product.ProductImageFile.CopyToAsync(fileStream);
                        }

                        //delete image from wwwroot/image
                        
                    }
                    catch (Exception e)
                    {
                        ViewBag.Message = e.ToString();
                    }
                    

                    
                    
                    _context.Update(product);
                    await _context.SaveChangesAsync();
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
            ViewData["CategoryId"] = new SelectList(_context.CategorieDbSet, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["TypeId"] = new SelectList(_context.TypeDbSet, "TypeId", "TypeName", product.TypeId);
            return View(product);
        }

        // GET: Product/Delete/5
        [Authorize(Roles = "Admin, Manager")]
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.ProductDbSet
                .Include(p => p.Category)
                .Include(p => p.Type)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [Authorize(Roles = "Admin, Manager")]
        [NoDirectAccess]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.ProductDbSet.FindAsync(id);

            //delete image from wwwroot/image
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", product.ProductImage);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            _context.ProductDbSet.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ProductExists(int id)
        {
            return _context.ProductDbSet.Any(e => e.ProductId == id);
        }
    }
}
