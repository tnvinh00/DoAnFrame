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
    public class OrderDetailController : Controller
    {
        private readonly SkyAppDbContext _context;
        private readonly UserManager<SkyUser> _userManager;

        public OrderDetailController(SkyAppDbContext context,
            UserManager<SkyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [NoDirectAccess]
        public async Task<IActionResult> ChiTiet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var iduser = _userManager.GetUserId(User);

                var order = _context.OrderDbSet.Where(c => c.OrderId == id).ToList().FirstOrDefault();

                var orderDetail = await _context.OrderDetailDbSet
                    .Include(o => o.Order)
                    .Include(o => o.Product).Where(c => c.OrderId == id).ToListAsync();

                if (orderDetail == null)
                {
                    return NotFound();
                }

                ViewBag.orderIdText = order.OrderTextId;

                ViewBag.orderPrice = order.OrderPrice;

                ViewBag.order = order;

                return View(orderDetail);
            }
            catch
            {
                ViewBag.Mess = "Đã có lỗi";
            }

            return View();

        }

        /*
        [Authorize]
        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            var id = _userManager.GetUserId(User);


            var skyAppDbContext = _context.OrderDetailDbSet.Include(o => o.Order).Include(o => o.Product);
            return View(await skyAppDbContext.ToListAsync());
        }

        // GET: OrderDetail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetailDbSet
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // GET: OrderDetail/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.OrderDbSet, "OrderId", "OrderNote");
            ViewData["ProductId"] = new SelectList(_context.ProductDbSet, "ProductId", "ProductDescription");
            return View();
        }

        // POST: OrderDetail/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ProductId,ProductOrderName,ProductOrderPrice,Quantity")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.OrderDbSet, "OrderId", "OrderNote", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.ProductDbSet, "ProductId", "ProductDescription", orderDetail.ProductId);
            return View(orderDetail);
        }

        [NoDirectAccess]
        // GET: OrderDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetailDbSet.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.OrderDbSet, "OrderId", "OrderNote", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.ProductDbSet, "ProductId", "ProductDescription", orderDetail.ProductId);
            return View(orderDetail);
        }

        // POST: OrderDetail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,ProductId,ProductOrderName,ProductOrderPrice,Quantity")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.OrderId))
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
            ViewData["OrderId"] = new SelectList(_context.OrderDbSet, "OrderId", "OrderNote", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.ProductDbSet, "ProductId", "ProductDescription", orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetailDbSet
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetailDbSet.FindAsync(id);
            _context.OrderDetailDbSet.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetailDbSet.Any(e => e.OrderId == id);
        }
    }
}
