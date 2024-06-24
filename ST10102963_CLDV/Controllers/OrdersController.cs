using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.EntityFrameworkCore;
using ST10102963_CLDV.Data;
using ST10102963_CLDV.Models;

namespace ST10102963_CLDV.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ST10102963_CLDVContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IDurableOrchestrationClient _orchestratorClient;

        public OrdersController(ST10102963_CLDVContext context, UserManager<IdentityUser> userManager, IDurableOrchestrationClient orchestratorClient)
        {
            _context = context;
            _userManager = userManager;
            _orchestratorClient = orchestratorClient;
        }

        // GET: Orders
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<OrdersViewModel> order = _context.OrdersViewModel.ToList();
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentUser.UserName;
            var userEmail = currentUser?.Email; // Get user's email if logged in

            IEnumerable<OrdersViewModel> orders;

            if (User.IsInRole("Admin"))
            {
                // Admins see all orders
                orders = await _context.OrdersViewModel.ToListAsync();
            }
            else
            {
                // Clients see their orders or orders without email (if applicable)
                orders = await _context.OrdersViewModel.Where(o => userEmail == null || o.UserEmail == userEmail).ToListAsync();
            }

            /* Code reference: https://youtu.be/O57nsLyZubc?si=dFWp2kaoQykl0Aqm */
            const int pageSize = 5;
            if (pg < 1)
                pg = 1;

            int recsCount = order.Count();
            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;
            var data = order.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            await _context.OrdersViewModel.ToListAsync();

            return View(data);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrdersViewModel == null)
            {
                return NotFound();
            }

            var ordersViewModel = await _context.OrdersViewModel
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (ordersViewModel == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentUser.UserName;

            return View(ordersViewModel);
        }

        // GET: Orders/Create

        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,OrderDate,TotalAmount")] OrdersViewModel ordersViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordersViewModel);
                await _context.SaveChangesAsync();

                var currentUser = await _userManager.GetUserAsync(User);
                var userEmail = currentUser?.Email; // Get user's email if logged in

                var order = new OrdersViewModel
                {
                    OrderDate = DateTime.Now,
                    TotalAmount = ordersViewModel.TotalAmount,
                    UserEmail = userEmail
                    // Map other properties as needed
                };
                await _orchestratorClient.StartNewAsync("OrderOrchestrator", order);


                // Start the orchestrator after saving the order
                return RedirectToAction(nameof(Index));
            }
            return View(ordersViewModel);
        }

        // GET: Orders/Edit/5
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrdersViewModel == null)
            {
                return NotFound();
            }

            var ordersViewModel = await _context.OrdersViewModel.FindAsync(id);
            if (ordersViewModel == null)
            {
                return NotFound();
            }
            return View(ordersViewModel);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,OrderDate,TotalAmount")] OrdersViewModel ordersViewModel)
        {
            if (id != ordersViewModel.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordersViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersViewModelExists(ordersViewModel.OrderID))
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
            return View(ordersViewModel);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrdersViewModel == null)
            {
                return NotFound();
            }

            var ordersViewModel = await _context.OrdersViewModel
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (ordersViewModel == null)
            {
                return NotFound();

                var currentUser = await _userManager.GetUserAsync(User);
                ViewBag.CurrentUser = currentUser.UserName;
            }

            return View(ordersViewModel);
        }

        // POST: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrdersViewModel == null)
            {
                return Problem("Entity set 'ST10102963_CLDVContext.OrdersViewModel'  is null.");
            }
            var ordersViewModel = await _context.OrdersViewModel.FindAsync(id);
            if (ordersViewModel != null)
            {
                _context.OrdersViewModel.Remove(ordersViewModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersViewModelExists(int id)
        {
          return (_context.OrdersViewModel?.Any(e => e.OrderID == id)).GetValueOrDefault();
        }



    }
}
