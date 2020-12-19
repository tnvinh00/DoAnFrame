using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sky.Areas.Identity.Data;
using Sky.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sky.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ReportController : Controller
    {
        private readonly SkyAppDbContext _context;

        public ReportController(SkyAppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Index(int year = 0)
        {
            //Thống kê doanh thu
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }
            //Năm
            ViewData["Year"] = year;

            var skyAppDbContext = _context.OrderDbSet
                .Where(c => c.OrderStatus == "Đã giao" && c.OrderDate.Year == year)
                .GroupBy(c => c.OrderDate.Month)
                .Select(c => new { Month = c.Key, Revenue = c.Sum(d => d.OrderPrice) })
                .OrderBy(c => c.Month)
                .ToList();

            int[] arr = new int[12];

            foreach (var item in skyAppDbContext)
            {
                arr[item.Month - 1] = item.Revenue;
            }
            //Doanh thu theo tháng
            ViewBag.Revenue = arr;


            //Thống kê
            ViewBag.sum = arr.Sum();

            return View();
        }
    }
}
