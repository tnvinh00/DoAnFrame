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
    public class FavoriteDetailController : Controller
    {
        private readonly SkyAppDbContext _context;
        private readonly UserManager<SkyUser> _userManager;
        private readonly SignInManager<SkyUser> _signInManager;

        public FavoriteDetailController(SkyAppDbContext context,
            UserManager<SkyUser> userManager,
            SignInManager<SkyUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [NoDirectAccess]
        [Authorize]
        public async Task<IActionResult> Favorite_Partial()
        {
            var id = _userManager.GetUserId(User);

            var favo = _context.FavoriteDbSet.Where(c => c.UserId == id).ToList().FirstOrDefault();

            var skyAppDbContext = _context.FavoriteDetailDbSet.Include(c => c.Favorite).Include(c => c.Product).Where(c => c.FavoriteId == favo.FavoriteId);

            ViewData["More"] = "";

            if (!skyAppDbContext.Any())
            {
                ViewData["FavoriteCount"] = "Bạn chưa có sản phẩm trong yêu thích";
            }

            if (skyAppDbContext.Count() > 5)
            {
                ViewData["More"] = "...";
            }

            ViewData["FavoriteCount"] = "Yêu thích có " + skyAppDbContext.Count() + " sản phẩm";

            return View(await skyAppDbContext.Take(5).ToListAsync());
        }

        [Authorize]
        [NoDirectAccess]
        public async Task<IActionResult> AddtoFavo(int? id)
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

                var favo = _context.FavoriteDbSet.Where(c => c.UserId == iduser).ToList().FirstOrDefault();

                var check = _context.FavoriteDetailDbSet.Where(c => c.FavoriteId == favo.FavoriteId && c.ProductId == id).FirstOrDefault();

                if (check != null)
                {
                    ViewData["Mess"] = "Sản phẩm đã có trong danh sách yêu thích!";
                    return View();
                }

                var favode = new FavoriteDetail { FavoriteId = favo.FavoriteId, ProductId = (int)id };

                _context.FavoriteDetailDbSet.Add(favode);

                await _context.SaveChangesAsync();

                ViewData["Mess"] = "Thêm thành công";
            }
            catch
            {
                ViewData["Mess"] = "Thất bại";
            }
            return View();
        }


        [Authorize]
        [NoDirectAccess]
        public async Task<IActionResult> DeleteItemFavo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var iduser = _userManager.GetUserId(User);
            try
            {
                var favorite = _context.FavoriteDbSet.Where(c => c.UserId == iduser).ToList().FirstOrDefault();

                var favoritede = new FavoriteDetail { FavoriteId = favorite.FavoriteId, ProductId = (int)id };

                _context.FavoriteDetailDbSet.Remove(favoritede);

                await _context.SaveChangesAsync();

                ViewData["Mess"] = "Đã xóa";
            }
            catch
            {
                ViewData["Mess"] = "Thất bại";
            }
            return View();
            //KO return để ko hiển thị
        }


        [Authorize]
        // GET: FavoriteDetail
        public async Task<IActionResult> Index()
        {
            var id = _userManager.GetUserId(User);

            var favo = _context.FavoriteDbSet.Where(c => c.UserId == id).ToList().FirstOrDefault();

            var skyAppDbContext = _context.FavoriteDetailDbSet.Include(c => c.Favorite).Include(c => c.Product).Include(c => c.Product.Type).Include(c => c.Product.Category).Where(c => c.FavoriteId == favo.FavoriteId);

            return View(await skyAppDbContext.ToListAsync());
        }
    }
}
