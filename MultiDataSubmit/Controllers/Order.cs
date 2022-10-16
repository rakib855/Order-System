using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MultiDataSubmit.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MultiDataSubmit.Controllers
{
    public class Order : Controller
    {
        private readonly MultiDataDB _context;

        public Order(MultiDataDB context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            var applicationDbContext = _context.Orders.Include(o => o.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(o => o.Customer).ThenInclude(c => c.City).ThenInclude(n => n.Country).Include(i => i.OrderItems).ThenInclude(p => p.Product).FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var Product = _context.Products.ToList();
            ViewData["Products"] = new SelectList(Product, "ID", "ProductName");
            var customers = _context.Customers.Select(e => new { e.ID, Name = e.FirstName + " " + e.LastName }).ToList();
            ViewData["CustomerId"] = new SelectList(customers, "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,OrderDate,OrderNumber,CustomerId,TotalAmount,OrderItems")] Models.Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var Product = _context.Products.ToList();
            ViewData["Products"] = new SelectList(Product, "ID", "ProductName");
            var customers = _context.Customers.Select(e => new { e.ID, Name = e.FirstName + " " + e.LastName });
            ViewData["CustomerId"] = new SelectList(_context.Customers, "ID", "Name", order.CustomerId);
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(e => e.OrderItems).SingleAsync(e => e.ID == id);
            if (order == null)
            {
                return NotFound();
            }
            var Product = _context.Products.Include(e => e.OrderItems).ToList();
            ViewData["Products"] = new SelectList(Product, "ID", "ProductName");
            var customers = _context.Customers.Select(e => new { e.ID, Name = e.FirstName + " " + e.LastName });
            ViewData["CustomerId"] = new SelectList(customers, "ID", "Name", order.CustomerId);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ID,OrderDate,OrderNumber,CustomerId,TotalAmount,OrderItems")] Models.Order order)
        {
            if (id != order.ID)
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
                    if (!OrderExists(order.ID))
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
            var Product = _context.Products.ToList();
            ViewData["Products"] = new SelectList(Product, "ID", "ProductName");
            var customers = _context.Customers.Select(e => new { e.ID, Name = e.FirstName + " " + e.LastName });
            ViewData["CustomerId"] = new SelectList(customers, "ID", "Name", order.CustomerId);
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(o => o.Customer).ThenInclude(c => c.City).ThenInclude(n => n.Country).Include(i => i.OrderItems).ThenInclude(p => p.Product).FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public JsonResult GetUnitPrice(long prodId)
        {
            return Json(_context.Products.Find(prodId).UnitPrice);
        }

        private bool OrderExists(long id)
        {
            return _context.Orders.Any(e => e.ID == id);
        }
    }
}
