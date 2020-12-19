using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class OrderController : Controller
    {
        private readonly SkyAppDbContext _context;
        private readonly UserManager<SkyUser> _userManager;

        public OrderController(SkyAppDbContext context,
            UserManager<SkyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string StatusMessage { set; get; }

        [Authorize]
        // GET: Order
        public async Task<IActionResult> Index(string searchString, string status)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["Status"] = status;

            var iduser = _userManager.GetUserId(User);
            try
            {
                var skyAppDbContext = _context.OrderDbSet.Where(c => c.UserId == iduser);

                if (!String.IsNullOrEmpty(searchString))
                {
                    skyAppDbContext = skyAppDbContext.Where(s => s.OrderTextId.Contains(searchString));
                }



                switch (status)
                {
                    case "dang-cho":
                        skyAppDbContext = skyAppDbContext.Where(s => s.OrderStatus.Contains("Đang chờ duyệt"));
                        break;
                    case "da-xac-nhan":
                        skyAppDbContext = skyAppDbContext.Where(s => s.OrderStatus.Contains("Đã xác nhận"));
                        break;
                    case "dang-giao":
                        skyAppDbContext = skyAppDbContext.Where(s => s.OrderStatus.Contains("Đang giao"));
                        break;
                    case "da-giao":
                        skyAppDbContext = skyAppDbContext.Where(s => s.OrderStatus.Contains("Đã giao"));
                        break;
                    case "da-huy":
                        skyAppDbContext = skyAppDbContext.Where(s => s.OrderStatus.Contains("Đã hủy"));
                        break;
                    default:
                        skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.OrderDate);
                        break;
                }

                if (!skyAppDbContext.Any())
                {
                    ViewData["Mess"] = "Không có đơn hàng hợp lệ!";
                }

                //Lấy hình ảnh thum của từng đơn hàng
                List<string> listimg = new List<string>();

                foreach (var i in skyAppDbContext.ToList())
                {

                    var order_de = _context.OrderDetailDbSet.Where(c => c.OrderId == i.OrderId).ToList().FirstOrDefault();

                    var product = _context.ProductDbSet.Find(order_de.ProductId);

                    listimg.Add(product.ProductImage);
                }

                ViewBag.ListImg = listimg;

                return View(await skyAppDbContext.ToListAsync());
            }
            catch
            {
                ViewData["Mess"] = "Đã có lỗi!";
            }
            return View();
        }

        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Manage(string searchString, string status)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["Status"] = status;

            try
            {
                var skyAppDbContext = _context.OrderDbSet.Select(a => a);

                if (!String.IsNullOrEmpty(searchString))
                {
                    skyAppDbContext = skyAppDbContext.Where(s => s.OrderTextId.Contains(searchString));
                }



                switch (status)
                {
                    case "dang-cho":
                        skyAppDbContext = skyAppDbContext.Where(s => s.OrderStatus.Contains("Đang chờ duyệt"));
                        break;
                    case "da-xac-nhan":
                        skyAppDbContext = skyAppDbContext.Where(s => s.OrderStatus.Contains("Đã xác nhận"));
                        break;
                    case "dang-giao":
                        skyAppDbContext = skyAppDbContext.Where(s => s.OrderStatus.Contains("Đang giao"));
                        break;
                    case "da-giao":
                        skyAppDbContext = skyAppDbContext.Where(s => s.OrderStatus.Contains("Đã giao"));
                        break;
                    case "da-huy":
                        skyAppDbContext = skyAppDbContext.Where(s => s.OrderStatus.Contains("Đã hủy"));
                        break;
                    default:
                        skyAppDbContext = skyAppDbContext.OrderByDescending(s => s.OrderDate);
                        break;
                }

                if (!skyAppDbContext.Any())
                {
                    ViewData["Mess"] = "Không có đơn hàng hợp lệ!";
                }

                //Lấy hình ảnh thum của từng đơn hàng
                List<string> listimg = new List<string>();

                foreach (var i in skyAppDbContext.ToList())
                {

                    var order_de = _context.OrderDetailDbSet.Where(c => c.OrderId == i.OrderId).ToList().FirstOrDefault();

                    var product = _context.ProductDbSet.Find(order_de.ProductId);

                    listimg.Add(product.ProductImage);
                }

                ViewBag.ListImg = listimg;

                return View(await skyAppDbContext.ToListAsync());
            }
            catch
            {
                ViewData["Mess"] = "Đã có lỗi!";
            }
            return View();
        }

        [NoDirectAccess]
        [Authorize]
        public async Task<IActionResult> AcceptOrder(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var skyAppDbContext = _context.OrderDbSet.Select(a => a).Where(c => c.OrderStatus == "Đang chờ duyệt");

            if (!String.IsNullOrEmpty(searchString))
            {
                skyAppDbContext = skyAppDbContext.Where(s => s.OrderTextId.Contains(searchString));
            }

            if (!skyAppDbContext.Any())
            {
                ViewData["Mess"] = "Không có đơn hàng hợp lệ!";
            }

            skyAppDbContext = skyAppDbContext.OrderByDescending(c => c.OrderDate);

            return View(await skyAppDbContext.ToListAsync());
        }

        [NoDirectAccess]
        [Authorize]
        public IActionResult AcceptOrderConfirmed(int id)
        {
            var skyAppDbContext = _context.OrderDbSet.Where(c => c.OrderId == id).FirstOrDefault();

            skyAppDbContext.OrderStatus = "Đã xác nhận";
            skyAppDbContext.OrderLock = true;

            _context.SaveChanges();

            return RedirectToAction(nameof(AcceptOrder));
        }

        [NoDirectAccess]
        [Authorize]
        public IActionResult DeleteOrderCancel()
        {
            var skyAppDbContext = _context.OrderDbSet.Where(c => c.OrderStatus == "Đã hủy").ToList();

            return View(skyAppDbContext);
        }

        //Hủy đơn hàng
        [Authorize]
        [NoDirectAccess]
        public IActionResult CancelOrder(int id)
        {
            var iduser = _userManager.GetUserId(User);

            var skyAppDbContext = _context.OrderDbSet.Where(c => c.UserId == iduser && c.OrderId == id).FirstOrDefault();

            ViewData["Mess"] = "Bạn có muốn hủy đơn hàng " + skyAppDbContext.OrderTextId + " không?";

            ViewBag.Id = id;

            return View();
        }

        [Authorize]
        [NoDirectAccess]
        public IActionResult CancelOrderConfirmed(int id)
        {
            try
            {
                var iduser = _userManager.GetUserId(User);

                var skyAppDbContext = _context.OrderDbSet.Where(c => c.UserId == iduser && c.OrderId == id).FirstOrDefault();

                skyAppDbContext.OrderStatus = "Đã hủy";

                _context.SaveChanges();

                ViewData["Mess"] = "Hủy thành công";
            }
            catch
            {
                ViewData["Mess"] = "Hủy không thành công";
            }

            return RedirectToAction("Index");
        }

        [NoDirectAccess]
        [Authorize]
        //Waiting success
        public IActionResult Success()
        {
            ViewBag.text = HttpContext.Session.GetString("Status");

            //ViewBag.text = StatusMessage;

            return View();
        }
        [Authorize]
        [NoDirectAccess]
        public async Task<IActionResult> DeleteOrderCancelConfirmed(int id)
        {
            var order = await _context.OrderDbSet.FindAsync(id);
            _context.OrderDbSet.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DeleteOrderCancel));
        }
        public async Task<IActionResult> DeleteAllOrderCancel()
        {
            var order = await _context.OrderDbSet.Where(c => c.OrderStatus == "Đã hủy").ToListAsync();

            foreach (var item in order){
                _context.OrderDbSet.Remove(item);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DeleteOrderCancel));
        }

        [Authorize]
        [NoDirectAccess]
        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.OrderDbSet
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize]
        [NoDirectAccess]
        // GET: Order/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize]
        [HttpPost]
        [NoDirectAccess]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ReciverName,ReciverAddress,ReciverPhone,ReciverEmail,OrderTextId,OrderStatus,OrderLock,OrderNote,OrderDate,OrderPrice,UserId")] Order order)
        {
            var id = _userManager.GetUserId(User);

            int price = 0;

            var cart = _context.CartDbSet.Where(c => c.UserId == id).ToList().FirstOrDefault();

            var skyAppDbContext = _context.CartDetailDbSet.Include(c => c.Cart).Include(c => c.Product).Where(c => c.CartId == cart.CartId).ToList();

            //tính tổng thành tiền
            foreach (var item in skyAppDbContext)
            {
                price += item.Product.ProductPrice * item.ProductQuantity;
            }

            order.OrderTextId = id.Substring(0, 6) + "-" + DateTime.Now.ToString("ddMMyyHHmmssff");
            order.OrderDate = DateTime.Now;
            order.UserId = id;
            order.OrderPrice = price;
            order.OrderStatus = "Đang chờ duyệt";
            order.OrderLock = false;

            //tạo đơn hàng
            _context.Add(order);
            await _context.SaveChangesAsync();

            //thêm sản phẩm vào chi tiết đơn hàng
            foreach (var item in skyAppDbContext)
            {
                price += item.Product.ProductPrice;

                var order_de = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    ProductOrderName = item.Product.ProductName,
                    ProductOrderPrice = item.Product.ProductPrice.ToString(),
                    Quantity = item.ProductQuantity
                };
                _context.OrderDetailDbSet.Add(order_de);
                await _context.SaveChangesAsync();
            }
            //Xóa item trong giỏ hàng
            var ca = _context.CartDetailDbSet.Where(c => c.CartId == cart.CartId);

            foreach (var a in ca)
            {
                _context.CartDetailDbSet.Remove(a);

            }
            await _context.SaveChangesAsync();

            string tus = order.OrderTextId;

            //StatusMessage = tus;

            HttpContext.Session.SetString("Status", tus);

            return RedirectToAction(nameof(Success));
        }

        // GET: Order/Edit/5
        [Authorize]
        [NoDirectAccess]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.OrderDbSet.FindAsync(id);

            IEnumerable<string> names = new List<string>() { "Đang chờ duyệt", "Đã xác nhận", "Đang giao", "Đã giao", "Đã hủy" };

            ViewData["Status"] = new SelectList(names);

            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [NoDirectAccess]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,ReciverName,ReciverAddress,ReciverPhone,OrderLock,ReciverEmail,OrderTextId,OrderStatus,OrderNote,OrderDate,OrderPrice,UserId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Manage));
            }
            return View(order);
        }

        // GET: Order/Edit/5
        [Authorize]
        [NoDirectAccess]
        public async Task<IActionResult> EditforUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var iduser = _userManager.GetUserId(User);

            var order = await _context.OrderDbSet.FindAsync(id);

            if (order.UserId != iduser)
            {
                return NotFound();
            }
            IEnumerable<string> names = new List<string>() { "Đang chờ duyệt", "Đã xác nhận", "Đang giao", "Đã giao", "Đã hủy" };

            ViewData["Status"] = new SelectList(names);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [NoDirectAccess]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditforUser(int id, [Bind("OrderId,ReciverName,ReciverAddress,ReciverPhone,ReciverEmail,OrderTextId,OrderStatus,OrderNote,OrderDate,OrderPrice,UserId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Manage));
            }
            return View(order);
        }

        // GET: Order/Delete/5
        [NoDirectAccess]
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.OrderDbSet
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [NoDirectAccess]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.OrderDbSet.FindAsync(id);
            _context.OrderDbSet.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        

        private bool OrderExists(int id)
        {
            return _context.OrderDbSet.Any(e => e.OrderId == id);
        }
    }
}
