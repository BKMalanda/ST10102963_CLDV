using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10102963_CLDV.Data;
using ST10102963_CLDV.Models;

namespace ST10102963_CLDV.Controllers
{
    
    public class ProductsController : Controller
    {
        private readonly SearchClient _searchClient;
        private readonly ST10102963_CLDVContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductsController(IConfiguration configuration, ST10102963_CLDVContext context, UserManager<IdentityUser> userManager)
        {
            /* Code reference: https://www.youtube.com/watch?v=5_29PeUorms */

            string searchServiceEndpoint = configuration["AzureSearch:Endpoint"];
            string indexName = configuration["AzureSearch:IndexName"];
            string queryApiKey = configuration["AzureSearch:QueryApiKey"];
            _searchClient = new SearchClient(new Uri(searchServiceEndpoint), indexName, new AzureKeyCredential(queryApiKey));
            _context = context;
            _userManager = userManager;
          
        }

        // GET: Products
        public async Task<IActionResult> Index(int pg=1)
        {
            List<ProductsViewModel> products = _context.ProductsViewModel.ToList();
            var sT10102963_CLDVContext = _context.ProductsViewModel.Include(p => p.CategoriesViewModel);

            /* Code reference: https://youtu.be/O57nsLyZubc?si=dFWp2kaoQykl0Aqm */
            const int pageSize = 5;
            if (pg < 1)
                pg = 1;

            int recsCount = products.Count();
            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;
            var data = products.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            await sT10102963_CLDVContext.OrderByDescending(p => p.ProductID).ToListAsync();

            return View(data);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductsViewModel == null)
            {
                return NotFound();
            }

            var productsViewModel = await _context.ProductsViewModel
                .Include(p => p.CategoriesViewModel)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (productsViewModel == null)
            {
                return NotFound();
            }

            return View(productsViewModel);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.CategoriesViewModel, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,Price,CategoryID,Availability")] ProductsViewModel productsViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productsViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.CategoriesViewModel, "CategoryID", "CategoryName", productsViewModel.CategoryID);
            return View(productsViewModel);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductsViewModel == null)
            {
                return NotFound();
            }

            var productsViewModel = await _context.ProductsViewModel.FindAsync(id);
            if (productsViewModel == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.CategoriesViewModel, "CategoryID", "CategoryName", productsViewModel.CategoryID);
            return View(productsViewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductName,Price,CategoryID,Availability")] ProductsViewModel productsViewModel)
        {
            if (id != productsViewModel.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productsViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsViewModelExists(productsViewModel.ProductID))
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
            ViewData["CategoryID"] = new SelectList(_context.CategoriesViewModel, "CategoryID", "CategoryName", productsViewModel.CategoryID);
            return View(productsViewModel);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductsViewModel == null)
            {
                return NotFound();
            }

            var productsViewModel = await _context.ProductsViewModel
                .Include(p => p.CategoriesViewModel)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (productsViewModel == null)
            {
                return NotFound();
            }

            return View(productsViewModel);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductsViewModel == null)
            {
                return Problem("Entity set 'ST10102963_CLDVContext.ProductsViewModel'  is null.");
            }
            var productsViewModel = await _context.ProductsViewModel.FindAsync(id);
            if (productsViewModel != null)
            {
                _context.ProductsViewModel.Remove(productsViewModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsViewModelExists(int id)
        {
          return (_context.ProductsViewModel?.Any(e => e.ProductID == id)).GetValueOrDefault();
        }

        // In ProductsController

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")] // Allow only clients to place orders
        public async Task<IActionResult> PlaceOrder(int productId)
        {
            var product = await _context.ProductsViewModel.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var userEmail = currentUser?.Email; // Get user's email if logged in

            var quantity = 1;

            var newOrder = new OrdersViewModel
            {
                OrderDate = DateTime.Now,
                TotalAmount = product.Price * quantity,
                UserEmail = userEmail
            };

            _context.Add(newOrder);
            await _context.SaveChangesAsync();



            // Now that the order is saved, we can create order details
            var newOrderDetail = new OrderDetailsViewModel
            {
                OrderID = newOrder.OrderID,
                ProductID = product.ProductID,
                Quantity = 1, // Assuming quantity is 1 for now
                UnitPrice = product.Price,
                OrderStatusID = 1, // Set the default order status ID
                UserEmail = userEmail
            };

            _context.Add(newOrderDetail);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "OrderDetails", new { id = newOrder.OrderID });
        }

        /* Code reference: https://stackoverflow.com/ */
        /* Code reference: https://youtu.be/O57nsLyZubc?si=dFWp2kaoQykl0Aqm */
      /*  Searching documents:
        The search functionality is based on the examples in the Azure Cognitive Search documentation.
        Reference: https://learn.microsoft.com/en-us/azure/search/search-query-create-dotnet */

        public async Task<IActionResult> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return View(await _context.ProductsViewModel.ToListAsync());
            }

            var options = new SearchOptions
            {
                Filter = "",
                Size = 1000
            };

            SearchResults<ProductsViewModel> results = await _searchClient.SearchAsync<ProductsViewModel>(searchText, options);

            return View("Index", results.GetResults().Select(r => r.Document).ToList());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSearchIndex()
        {
            await UploadProducts();
            return RedirectToAction("Index", "Product", new { message = "Search index updated successfully" });
        }

        private async Task UploadProducts()
        {
            var products = await _context.ProductsViewModel.ToListAsync();
            var batch = new IndexDocumentsBatch<ProductsViewModel>();
            foreach (var product in products)
            {
                batch.Actions.Add(IndexDocumentsAction.Upload(product));
            }
            await _searchClient.IndexDocumentsAsync(batch);
        }
    }
}

