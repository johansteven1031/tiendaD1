using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public JsonResult finishShoppingDetails()
        {
            try
            {
                string UserId = HttpContext.User.Identities.First().Claims.First().Value;

                //Se revisa que el usuario no tenga orden de compra abierta
                List<Orders> orders = _context.Orders.Where(f => f.UserId == UserId && f.Finished != true).ToList();
                //Si no existe la orden crearla


                if (orders.Count > 0)
                {
                    orders.First().Finished = true;
                    _context.Orders.Attach(orders.First());
                    _context.SaveChanges();
                }

                return this.Json("Pedido Finalizado Satisfactoriamente");

            }
            catch (Exception ex)
            {
                return this.Json(ex.Message.ToString());
            }
        }
        public JsonResult deleteorderDetails(int OrderDetailId)
        {
            try
            {
                OrderDetails order_detail = _context.OrderDetails.Where(f => f.OrderDetailId == OrderDetailId).FirstOrDefault();
                //Si no existe la orden crearla

                
                if (order_detail!=null)
                { 
                    _context.OrderDetails.Remove(order_detail);
                    _context.SaveChanges();
                }

                return this.Json("Eliminado Satisfactoriamente");

            }
            catch (Exception ex)
            {
                return this.Json(ex.Message.ToString());
            }
        }
        
        public JsonResult getShoppingDetails()
        {
            try
            { 
                string UserId = HttpContext.User.Identities.First().Claims.First().Value;
                
                //Se revisa que el usuario no tenga orden de compra abierta
                List<Orders> orders = _context.Orders.Where(f => f.UserId == UserId && f.Finished != true).ToList();
                //Si no existe la orden crearla

                List<OrderDetails> records = new List<OrderDetails>();
                if (orders.Count > 0)
                {
                    int OrderID = orders.First().OrderId;


                    records = _context.OrderDetails.Include("Product").Where(f => f.OrderId == OrderID).ToList();
                }

                return this.Json(records);

            }
            catch (Exception ex)
            {
                return null;
            } 
        }
        public JsonResult getQtyShopping()
        {
            try
            {

                string UserId = HttpContext.User.Identities.First().Claims.First().Value;
                int QtyShopping = 0;
                //Se revisa que el usuario no tenga orden de compra abierta
                List<Orders> orders =  _context.Orders.Where(f => f.UserId == UserId && f.Finished != true).ToList();
                //Si no existe la orden crearla
                if (orders.Count > 0)
                {
                    int OrderID = orders.First().OrderId;
                    List<OrderDetails> order_details=_context.OrderDetails.Where(f => f.OrderId == OrderID).ToList();
                    if (order_details.Count > 0)
                    {
                        QtyShopping = order_details.Sum(f => f.Quantity);
                    }
                }

                return this.Json(QtyShopping);

            }
            catch (Exception ex)
            {
                return null;
            }
            
        }


        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.OrderDetails.ToListAsync());
        }

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetails = await _context.OrderDetails
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);
            if (orderDetails == null)
            {
                return NotFound();
            }

            return View(orderDetails);
        }

        // GET: OrderDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderDetailId,Quantity,Cost,SubTotal,Creation")] OrderDetails orderDetails)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderDetails);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetails = await _context.OrderDetails.FindAsync(id);
            if (orderDetails == null)
            {
                return NotFound();
            }
            return View(orderDetails);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailId,Quantity,Cost,SubTotal,Creation")] OrderDetails orderDetails)
        {
            if (id != orderDetails.OrderDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailsExists(orderDetails.OrderDetailId))
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
            return View(orderDetails);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetails = await _context.OrderDetails
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);
            if (orderDetails == null)
            {
                return NotFound();
            }

            return View(orderDetails);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetails = await _context.OrderDetails.FindAsync(id);
            _context.OrderDetails.Remove(orderDetails);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailsExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailId == id);
        }
    }
}
