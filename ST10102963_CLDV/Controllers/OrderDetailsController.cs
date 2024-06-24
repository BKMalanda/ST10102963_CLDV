using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10102963_CLDV.Data;
using ST10102963_CLDV.Models;

namespace ST10102963_CLDV.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly ST10102963_CLDVContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderDetailsController(ST10102963_CLDVContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: OrderDetails
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<OrderDetailsViewModel> orderDetail = _context.OrderDetailsViewModel.ToList();


            var currentUser = await _userManager.GetUserAsync(User);
            var sT10102963_CLDVContext = _context.OrderDetailsViewModel.Include(o => o.OrderStatusViewModel).Include(o => o.OrdersViewModel).Include(o => o.ProductsViewModel);

            var userEmail = currentUser?.Email; // Get user's email if logged in

            IEnumerable<OrderDetailsViewModel> ordersDetails;

            if (User.IsInRole("Admin"))
            {
                // Admins see all orders
                ordersDetails = await _context.OrderDetailsViewModel.ToListAsync();
            }
            else
            {
                // Clients see their orders or orders without email (if applicable)
                ordersDetails = await _context.OrderDetailsViewModel.Where(o => userEmail == null || o.UserEmail == userEmail).ToListAsync();
            }


            /* Code reference: https://youtu.be/O57nsLyZubc?si=dFWp2kaoQykl0Aqm */
            const int pageSize = 5;
            if (pg < 1)
                pg = 1;

            int recsCount = orderDetail.Count();
            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;
            var data = orderDetail.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            await sT10102963_CLDVContext.ToListAsync();

            return View(data);
        }

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderDetailsViewModel == null)
            {
                return NotFound();
            }

            var orderDetailsViewModel = await _context.OrderDetailsViewModel
                .Include(o => o.OrderStatusViewModel)
                .Include(o => o.OrdersViewModel)
                .Include(o => o.ProductsViewModel)
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);
            if (orderDetailsViewModel == null)
            {
                return NotFound();
            }

            return View(orderDetailsViewModel);
        }

        // GET: OrderDetails/Create
        public IActionResult Create()
        {
            ViewData["OrderStatusID"] = new SelectList(_context.OrderStatusViewModel, "OrderStatusID", "StatusName");
            ViewData["OrderID"] = new SelectList(_context.OrdersViewModel, "OrderID", "OrderID");
            ViewData["ProductID"] = new SelectList(_context.ProductsViewModel, "ProductID", "ProductName");
            return View();
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderDetailID,OrderID,ProductID,Quantity,UnitPrice,OrderStatusID,UserEmail")] OrderDetailsViewModel orderDetailsViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetailsViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderStatusID"] = new SelectList(_context.OrderStatusViewModel, "OrderStatusID", "StatusName", orderDetailsViewModel.OrderStatusID);
            ViewData["OrderID"] = new SelectList(_context.OrdersViewModel, "OrderID", "OrderID", orderDetailsViewModel.OrderID);
            ViewData["ProductID"] = new SelectList(_context.ProductsViewModel, "ProductID", "ProductName", orderDetailsViewModel.ProductID);
            return View(orderDetailsViewModel);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderDetailsViewModel == null)
            {
                return NotFound();
            }

            var orderDetailsViewModel = await _context.OrderDetailsViewModel.FindAsync(id);
            if (orderDetailsViewModel == null)
            {
                return NotFound();
            }
            ViewData["OrderStatusID"] = new SelectList(_context.OrderStatusViewModel, "OrderStatusID", "StatusName", orderDetailsViewModel.OrderStatusID);
            ViewData["OrderID"] = new SelectList(_context.OrdersViewModel, "OrderID", "OrderID", orderDetailsViewModel.OrderID);
            ViewData["ProductID"] = new SelectList(_context.ProductsViewModel, "ProductID", "ProductName", orderDetailsViewModel.ProductID);
            return View(orderDetailsViewModel);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailID,OrderID,ProductID,Quantity,UnitPrice,OrderStatusID,UserEmail")] OrderDetailsViewModel orderDetailsViewModel)
        {
            if (id != orderDetailsViewModel.OrderDetailID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetailsViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailsViewModelExists(orderDetailsViewModel.OrderDetailID))
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
            ViewData["OrderStatusID"] = new SelectList(_context.OrderStatusViewModel, "OrderStatusID", "StatusName", orderDetailsViewModel.OrderStatusID);
            ViewData["OrderID"] = new SelectList(_context.OrdersViewModel, "OrderID", "OrderID", orderDetailsViewModel.OrderID);
            ViewData["ProductID"] = new SelectList(_context.ProductsViewModel, "ProductID", "ProductName", orderDetailsViewModel.ProductID);
            return View(orderDetailsViewModel);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderDetailsViewModel == null)
            {
                return NotFound();
            }

            var orderDetailsViewModel = await _context.OrderDetailsViewModel
                .Include(o => o.OrderStatusViewModel)
                .Include(o => o.OrdersViewModel)
                .Include(o => o.ProductsViewModel)
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);
            if (orderDetailsViewModel == null)
            {
                return NotFound();
            }

            return View(orderDetailsViewModel);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderDetailsViewModel == null)
            {
                return Problem("Entity set 'ST10102963_CLDVContext.OrderDetailsViewModel'  is null.");
            }
            var orderDetailsViewModel = await _context.OrderDetailsViewModel.FindAsync(id);
            if (orderDetailsViewModel != null)
            {
                _context.OrderDetailsViewModel.Remove(orderDetailsViewModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailsViewModelExists(int id)
        {
          return (_context.OrderDetailsViewModel?.Any(e => e.OrderDetailID == id)).GetValueOrDefault();
        }
    }
}
