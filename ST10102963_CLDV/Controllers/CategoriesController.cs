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
    public class CategoriesController : Controller
    {
        private readonly ST10102963_CLDVContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CategoriesController(ST10102963_CLDVContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: Categories
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<CategoriesViewModel> categories = _context.CategoriesViewModel.ToList();
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentUser.UserName;

            /* Code reference: https://youtu.be/O57nsLyZubc?si=dFWp2kaoQykl0Aqm */
            const int pageSize = 5;
            if (pg < 1)
                pg = 1;

            int recsCount = categories.Count();
            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;
            var data = categories.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            await _context.CategoriesViewModel.ToListAsync();


            return View(data);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CategoriesViewModel == null)
            {
                return NotFound();
            }

            var categoriesViewModel = await _context.CategoriesViewModel
                .FirstOrDefaultAsync(m => m.CategoryID == id);
            if (categoriesViewModel == null)
            {
                return NotFound();
            }

            return View(categoriesViewModel);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryID,CategoryName")] CategoriesViewModel categoriesViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoriesViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriesViewModel);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CategoriesViewModel == null)
            {
                return NotFound();
            }

            var categoriesViewModel = await _context.CategoriesViewModel.FindAsync(id);
            if (categoriesViewModel == null)
            {
                return NotFound();
            }
            return View(categoriesViewModel);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryID,CategoryName")] CategoriesViewModel categoriesViewModel)
        {
            if (id != categoriesViewModel.CategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriesViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriesViewModelExists(categoriesViewModel.CategoryID))
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
            return View(categoriesViewModel);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CategoriesViewModel == null)
            {
                return NotFound();
            }

            var categoriesViewModel = await _context.CategoriesViewModel
                .FirstOrDefaultAsync(m => m.CategoryID == id);
            if (categoriesViewModel == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentUser.UserName;

            return View(categoriesViewModel);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CategoriesViewModel == null)
            {
                return Problem("Entity set 'ST10102963_CLDVContext.CategoriesViewModel'  is null.");
            }
            var categoriesViewModel = await _context.CategoriesViewModel.FindAsync(id);
            if (categoriesViewModel != null)
            {
                _context.CategoriesViewModel.Remove(categoriesViewModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriesViewModelExists(int id)
        {
          return (_context.CategoriesViewModel?.Any(e => e.CategoryID == id)).GetValueOrDefault();
        }
    }
}
