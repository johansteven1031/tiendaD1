using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductoController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Lista
        public async Task<IActionResult> ListProducts()
        {
            if (User.Identity.Name != null)
            {
                ViewBag.Products = await _context.Producto.Where(f => f.Active == true).ToListAsync();

                return View();
            }
            else return Redirect("/Identity/Account/Login");
        }

        // POST: Producto/AddProductToCar/5/1
        [HttpPost, ActionName("AddProductToCar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductToCar(int ProductId, int Quantity)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Producto product = await _context.Producto.FindAsync(ProductId);

                int OrderId = 0;
                string UserId = HttpContext.User.Identities.First().Claims.First().Value;
                //Se revisa que el usuario no tenga orden de compra abierta
                List<Orders> orders = await _context.Orders.Where(f => f.UserId == UserId && f.Finished != true).ToListAsync();
                //Si no existe la orden crearla
                if (orders.Count == 0)
                {
                    Orders order = new Orders();
                    order.UserId = UserId;
                    order.Finished = false;
                    order.Creation = DateTime.Now;

                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();


                    OrderId = order.OrderId;
                }
                else
                {
                    OrderId = orders.First().OrderId;
                }


                List<OrderDetails> order_details = await _context.OrderDetails.Where(f => f.OrderId == OrderId).ToListAsync();

                int OrderDetailId = 0;
                if (order_details.Count > 0)
                {
                    if (order_details.Where(f => f.ProductId == ProductId).Count() > 0)
                    {
                        OrderDetailId = order_details.Where(f => f.ProductId == ProductId).First().OrderDetailId;
                    }
                }


                OrderDetails objDetails = null;
                //Si existe el producto, actualizar la cantidad
                if (OrderDetailId > 0)
                {
                    objDetails = await _context.OrderDetails.FindAsync(OrderDetailId);
                    objDetails.Quantity = objDetails.Quantity + Quantity;
                    objDetails.Cost = product.Cost;
                    objDetails.SubTotal = objDetails.Quantity * objDetails.Cost;

                    _context.OrderDetails.Attach(objDetails);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    objDetails = new OrderDetails();
                    objDetails.OrderId = OrderId;
                    objDetails.ProductId = ProductId;
                    objDetails.Quantity = objDetails.Quantity + Quantity;
                    objDetails.Cost = product.Cost;
                    objDetails.SubTotal = objDetails.Quantity * objDetails.Cost;
                    objDetails.Creation = DateTime.Now;

                    await _context.OrderDetails.AddAsync(objDetails);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }

            return RedirectToAction(nameof(ListProducts));
        }


        // GET: Producto
        public async Task<IActionResult> Index()
        {
            return View(await _context.Producto.ToListAsync());
        }

        // GET: Producto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Producto/Create
        public IActionResult Create()
        {
            ViewData["Marcas"] = new SelectList(_context.Marca, "Id", "Nombre");
            ViewData["Categorias"] = new SelectList(_context.Categoria, "Id", "Nombre");
            return View();
        }

        // POST: Producto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);
                Producto producto = new Producto
                {
                    Description = model.Description,
                    Code = model.Code,
                    Cost = model.Cost,
                    Active = model.Active,
                    Ruta = uniqueFileName,
                    MarcaId = model.MarcaId,
                    CategoriaId = model.CategoriaId
                };
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(new Producto());
        }

        // GET: Producto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: Producto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Description,Code,Cost,Active")] Producto producto)
        {
            if (id != producto.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.ProductId))
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
            return View(producto);
        }

        // GET: Producto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Producto.FindAsync(id);
            _context.Producto.Remove(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Producto.Any(e => e.ProductId == id);
        }
        private string UploadedFile(ProductoViewModel model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Image");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
