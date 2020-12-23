using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sky.Data;
using Sky.Models;
using static Sky.Data.Helper;

namespace Sky.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly SkyAppDbContext _context;

        public HomeController(ILogger<HomeController> logger,
            IWebHostEnvironment hostEnvironment,
            SkyAppDbContext context)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        public IActionResult Index()
        {
            string path = Path.Combine(_hostEnvironment.WebRootPath, "Panel");

            string[] filePaths1 = Directory.GetFiles(path, "*.png");

            string[] filePaths2 = Directory.GetFiles(path, "*.jpg");

            string[] filePaths3 = Directory.GetFiles(path, "*.jpeg");

            var filePaths = filePaths1.Union(filePaths2).Union(filePaths3).ToArray();

            List<string> img = new List<string>();

            foreach (var i in filePaths)
            {
                string temp = Path.GetFileName(i);
                img.Add(temp);
            }

            ViewBag.ImgList = img;

            return View();
        }

        [NoDirectAccess]
        public IActionResult Menu()
        {
            var sky = _context.CategorieDbSet.Select(c => c.CategoryName).ToList();

            sky.Remove("Nam");
            sky.Remove("Nữ");

            ViewBag.menu = sky;

            return View();
        }

        public IActionResult BestSale()
        {
            var sale = _context.OrderDetailDbSet
                .GroupBy(c => c.ProductId)
                .Select(a => new { ProductId = a.Key, SaleCount = a.Count() })

                .OrderByDescending(x => x.SaleCount).Take(10);

            var product = _context.ProductDbSet
                .Join(sale,
                    a => a.ProductId,
                    b => b.ProductId,
                    (a, b) => a
                )
                .ToList();

            return View(product);
        }

        [NoDirectAccess]
        public PartialViewResult BestView()
        {
            var best = _context.ProductDbSet.OrderByDescending(c => c.ViewCount).Take(10);

            return PartialView(best.ToList());
        }

        [NoDirectAccess]
        public IActionResult BestFavorite()
        {
            var favo = _context.FavoriteDetailDbSet
                .GroupBy(c => c.ProductId)
                .Select(a => new { ProductId = a.Key, FavoCount = a.Count() })

                .OrderByDescending(x => x.FavoCount).Take(10);

            var product = _context.ProductDbSet
                .Join(favo,
                    a => a.ProductId,
                    b => b.ProductId,
                    (a, b) => a
                )
                .ToList();

            return View(product.ToList());
        }
            
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Errors()
        {
            return View();
        }
    }
}
