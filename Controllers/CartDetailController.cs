using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class CartDetailController : Controller
    {
        private readonly SkyAppDbContext _context;
        private readonly UserManager<SkyUser> _userManager;

        public CartDetailController(SkyAppDbContext context,
            UserManager<SkyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [NoDirectAccess]
        [Authorize]
        public async Task<IActionResult> Cart_Partial()
        {
            var id = _userManager.GetUserId(User);

            var cart = _context.CartDbSet.Where(c => c.UserId == id).ToList().FirstOrDefault();

            var skyAppDbContext = _context.CartDetailDbSet.Include(c => c.Cart).Include(c => c.Product).Where(c => c.CartId == cart.CartId);

            ViewData["More"] = "";

            if (!skyAppDbContext.Any())
            {
                ViewData["CartCount"] = "Bạn chưa có sản phẩm trong giỏ hàng";
            }
            if (skyAppDbContext.Count() > 5)
            {
                ViewData["More"] = "...";
            }

            ViewData["CartCount"] = "Giỏ hàng có " + skyAppDbContext.Count() + " sản phẩm";

            return View(await skyAppDbContext.Take(5).ToListAsync());
        }

        [NoDirectAccess]
        [Authorize]
        public async Task<IActionResult> LoadCartAsync()
        {
            var id = _userManager.GetUserId(User);

            var cart = _context.CartDbSet.Where(c => c.UserId == id).ToList().FirstOrDefault();

            var skyAppDbContext = _context.CartDetailDbSet.Include(c => c.Cart).Include(c => c.Product).Where(c => c.CartId == cart.CartId);

            int total = 0;

            foreach (var i in skyAppDbContext.ToList())
            {
                total += i.Product.ProductPrice * i.ProductQuantity;
            }

            ViewBag.Total = total;

            return View(await skyAppDbContext.ToListAsync());
        }

        [NoDirectAccess]
        [Authorize]
        public async Task<IActionResult> DesItem(int? id)
        {
            var iduser = _userManager.GetUserId(User);

            try
            {
                var cart = _context.CartDbSet.Where(c => c.UserId == iduser).ToList().FirstOrDefault();

                var cartde = _context.CartDetailDbSet.Where(c => c.CartId == cart.CartId && c.ProductId == id).ToList().FirstOrDefault();

                if (cartde.ProductQuantity <= 1)
                {
                    ViewData["Mess"] = "Số lượng phải lớn hơn 1";
                    return View();
                }

                cartde.ProductQuantity -= 1;

                await _context.SaveChangesAsync();

                ViewData["Mess"] = "Giảm thành công";
            }
            catch
            {
                ViewData["Mess"] = "Thất bại";
            }

            return View();
        }

        [NoDirectAccess]
        [Authorize]
        public async Task<IActionResult> IncItem(int? id)
        {
            var iduser = _userManager.GetUserId(User);

            try
            {
                var cart = _context.CartDbSet.Where(c => c.UserId == iduser).ToList().FirstOrDefault();

                var cartde = _context.CartDetailDbSet.Where(c => c.CartId == cart.CartId && c.ProductId == id).ToList().FirstOrDefault();

                cartde.ProductQuantity += 1;

                await _context.SaveChangesAsync();

                ViewData["Mess"] = "Tăng thành công";
            }
            catch
            {
                ViewData["Mess"] = "Thất bại";
            }

            return View();
        }

        [NoDirectAccess]
        [Authorize]
        public async Task<IActionResult> DeleteItemCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var iduser = _userManager.GetUserId(User);
            try
            {
                var cart = _context.CartDbSet.Where(c => c.UserId == iduser).ToList().FirstOrDefault();

                var cartde = new CartDetail { CartId = cart.CartId, ProductId = (int)id };

                _context.CartDetailDbSet.Remove(cartde);

                await _context.SaveChangesAsync();

                ViewData["Mess"] = "Thành công";
            }
            catch
            {
                ViewData["Mess"] = "Thất bại";
            }
            return View();
            //KO return để ko hiển thị
        }




        [Authorize]
        // GET: CartDetail
        public async Task<IActionResult> Index()
        {
            var id = _userManager.GetUserId(User);

            var cart = _context.CartDbSet.Where(c => c.UserId == id).ToList().FirstOrDefault();

            var skyAppDbContext = _context.CartDetailDbSet.Include(c => c.Cart).Include(c => c.Product).Where(c => c.CartId == cart.CartId);

            int total = 0;

            foreach (var i in skyAppDbContext.ToList())
            {
                total += i.Product.ProductPrice * i.ProductQuantity;
            }

            ViewBag.Total = total;

            return View(await skyAppDbContext.ToListAsync());
        }
    }
}
