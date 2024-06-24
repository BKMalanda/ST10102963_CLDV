using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10102963_CLDV.Data;
using ST10102963_CLDV.Models;

namespace ST10102963_CLDV.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderStatusController : Controller
    {
        private readonly ST10102963_CLDVContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderStatusController(ST10102963_CLDVContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: OrderStatus
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<OrderStatusViewModel> orderStatus = _context.OrderStatusViewModel.ToList();

            /* Code reference: https://youtu.be/O57nsLyZubc?si=dFWp2kaoQykl0Aqm */
            const int pageSize = 5;
            if (pg < 1)
                pg = 1;

            int recsCount = orderStatus.Count();
            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;
            var data = orderStatus.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            await _context.OrderStatusViewModel.ToListAsync();

            return View(data);
        }

        // GET: OrderStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderStatusViewModel == null)
            {
                return NotFound();
            }

            var orderStatusViewModel = await _context.OrderStatusViewModel
                .FirstOrDefaultAsync(m => m.OrderStatusID == id);
            if (orderStatusViewModel == null)
            {
                return NotFound();
            }

            return View(orderStatusViewModel);
        }

        // GET: OrderStatus/Create

        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderStatusID,StatusName")] OrderStatusViewModel orderStatusViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderStatusViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderStatusViewModel);
        }

        // GET: OrderStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderStatusViewModel == null)
            {
                return NotFound();
            }

            var orderStatusViewModel = await _context.OrderStatusViewModel.FindAsync(id);
            if (orderStatusViewModel == null)
            {
                return NotFound();
            }
            return View(orderStatusViewModel);
        }

        // POST: OrderStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderStatusID,StatusName")] OrderStatusViewModel orderStatusViewModel)
        {
            if (id != orderStatusViewModel.OrderStatusID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderStatusViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderStatusViewModelExists(orderStatusViewModel.OrderStatusID))
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
            return View(orderStatusViewModel);
        }

        // GET: OrderStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderStatusViewModel == null)
            {
                return NotFound();
            }

            var orderStatusViewModel = await _context.OrderStatusViewModel
                .FirstOrDefaultAsync(m => m.OrderStatusID == id);
            if (orderStatusViewModel == null)
            {
                return NotFound();
            }

            return View(orderStatusViewModel);
        }

        // POST: OrderStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderStatusViewModel == null)
            {
                return Problem("Entity set 'ST10102963_CLDVContext.OrderStatusViewModel'  is null.");
            }
            var orderStatusViewModel = await _context.OrderStatusViewModel.FindAsync(id);
            if (orderStatusViewModel != null)
            {
                _context.OrderStatusViewModel.Remove(orderStatusViewModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderStatusViewModelExists(int id)
        {
          return (_context.OrderStatusViewModel?.Any(e => e.OrderStatusID == id)).GetValueOrDefault();
        }
    }
}
