using BibliotecaUVSucre.Data;
using BibliotecaUVSucre.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaUVSucre.Controllers
{
    [Authorize]
    public class LibrosController : Controller
    {
        private readonly AppDbContext _context;

        public LibrosController(AppDbContext context)
        {
            _context = context;
        }

        // Administrador y Bibliotecario: listado de gestión (con acciones editar/eliminar).
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Libros.OrderBy(l => l.Titulo).ToListAsync());
        }

        // Usuario: catálogo de solo lectura.
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Catalogo()
        {
            return View(await _context.Libros.OrderBy(l => l.Titulo).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Create([Bind("Titulo,Autor,Isbn,Editorial,Categoria,Anio,CantidadTotal,CantidadDisponible")] Libro libro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(libro);
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Autor,Isbn,Editorial,Categoria,Anio,CantidadTotal,CantidadDisponible")] Libro libro)
        {
            if (id != libro.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(libro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Libros.AnyAsync(l => l.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(libro);
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro != null)
            {
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
