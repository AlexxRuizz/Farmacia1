using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Farmacia1.Data;
using Farmacia1.Models;
using Rotativa.AspNetCore;
using System.Threading.Tasks;

namespace Farmacia1.Controllers
{
    [Authorize] // Protege todas las acciones en este controlador
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (TempData["MensajeBienvenida"] != null)
            {
                ViewBag.MensajeBienvenida = TempData["MensajeBienvenida"];
            }

            ViewBag.UserRole = TempData["UserRole"];
            ViewBag.UserName = TempData["UserName"];
            return View(await _context.Productos.ToArrayAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            var productoEncontrado = _context.Productos.Find(id);
            if (productoEncontrado != null)
            {
                return View(productoEncontrado);
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult Delete(int id)
        {
            var productoEncontrado = _context.Productos.Find(id);
            if (productoEncontrado != null)
            {
                return View(productoEncontrado);
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Ventas(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> RealizarVenta(int id, int CantidadVendida)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            if (CantidadVendida <= 0 || CantidadVendida > producto.Cantidad)
            {
                ModelState.AddModelError("", "Cantidad a vender no v�lida.");
                return View("Ventas", producto);
            }

            producto.Cantidad -= CantidadVendida;
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            TempData["ProductSold"] = "Venta realizada exitosamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPerson(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();
                TempData["ProductAdded"] = "Producto agregado exitosamente.";
                return RedirectToAction("Index");
            }
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> guardarCambio(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();
                TempData["ProductEdited"] = "Producto editado exitosamente.";
                return RedirectToAction("Index");
            }
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminarproducto(Producto producto)
        {
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                TempData["ProductDeleted"] = "Producto eliminado exitosamente.";
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> generarReporte()
        {
            return new ViewAsPdf("GenerateReport", await _context.Productos.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
