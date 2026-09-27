using BibliotecaUVSucre.Data;
using BibliotecaUVSucre.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaUVSucre.Controllers
{
    [Authorize]
    public class PrestamosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PrestamosController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Administrador y Bibliotecario: todos los préstamos del sistema.
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Index()
        {
            var prestamos = await _context.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.FechaPrestamo)
                .ToListAsync();
            return View(prestamos);
        }

        // Usuario: solo los préstamos que él mismo solicitó.
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> MisPrestamos()
        {
            var usuarioId = _userManager.GetUserId(User);
            var prestamos = await _context.Prestamos
                .Include(p => p.Libro)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaPrestamo)
                .ToListAsync();
            return View(prestamos);
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Create([Bind("LibroId,UsuarioId,FechaPrestamo,FechaDevolucionEsperada")] Prestamo prestamo)
        {
            var libro = await _context.Libros.FindAsync(prestamo.LibroId);
            if (libro == null || libro.CantidadDisponible < 1)
            {
                ModelState.AddModelError(string.Empty, "El libro seleccionado no tiene ejemplares disponibles.");
            }

            if (ModelState.IsValid && libro != null)
            {
                prestamo.Estado = EstadoPrestamo.Prestado;
                libro.CantidadDisponible -= 1;

                _context.Add(prestamo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarListas();
            return View(prestamo);
        }

        // Marca un préstamo como devuelto y repone el ejemplar al inventario.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Devolver(int id)
        {
            var prestamo = await _context.Prestamos.Include(p => p.Libro).FirstOrDefaultAsync(p => p.Id == id);
            if (prestamo == null) return NotFound();

            if (prestamo.Estado != EstadoPrestamo.Devuelto)
            {
                prestamo.Estado = EstadoPrestamo.Devuelto;
                prestamo.FechaDevolucionReal = DateTime.Now;
                if (prestamo.Libro != null)
                {
                    prestamo.Libro.CantidadDisponible += 1;
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void CargarListas()
        {
            ViewBag.Libros = new SelectList(
                _context.Libros.Where(l => l.CantidadDisponible > 0).OrderBy(l => l.Titulo), "Id", "Titulo");

            var usuarios = _context.Users.OrderBy(u => u.Nombre).ToList();
            ViewBag.Usuarios = new SelectList(usuarios, "Id", "Email");
        }
    }
}
