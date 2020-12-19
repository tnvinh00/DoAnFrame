using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sky.Data;
using Sky.Models;
using static Sky.Data.Helper;
using Type = Sky.Models.Type;

namespace Sky.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class TypeController : Controller
    {
        private readonly SkyAppDbContext _context;

        public TypeController(SkyAppDbContext context)
        {
            _context = context;
        }

        
        // GET: Type
        public async Task<IActionResult> Index()
        {
            return View(await _context.TypeDbSet.ToListAsync());
        }

        [NoDirectAccess]
        // GET: Type/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @type = await _context.TypeDbSet
                .FirstOrDefaultAsync(m => m.TypeId == id);
            if (@type == null)
            {
                return NotFound();
            }

            return View(@type);
        }

        [NoDirectAccess]
        // GET: Type/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Type/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [NoDirectAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeId,TypeName")] Type @type)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@type);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@type);
        }

        [NoDirectAccess]
        // GET: Type/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @type = await _context.TypeDbSet.FindAsync(id);
            if (@type == null)
            {
                return NotFound();
            }
            return View(@type);
        }

        [NoDirectAccess]
        // POST: Type/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TypeId,TypeName")] Type @type)
        {
            if (id != @type.TypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@type);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeExists(@type.TypeId))
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
            return View(@type);
        }

        [NoDirectAccess]
        // GET: Type/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @type = await _context.TypeDbSet
                .FirstOrDefaultAsync(m => m.TypeId == id);
            if (@type == null)
            {
                return NotFound();
            }

            return View(@type);
        }

        [NoDirectAccess]
        // POST: Type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @type = await _context.TypeDbSet.FindAsync(id);
            _context.TypeDbSet.Remove(@type);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TypeExists(int id)
        {
            return _context.TypeDbSet.Any(e => e.TypeId == id);
        }
    }
}
