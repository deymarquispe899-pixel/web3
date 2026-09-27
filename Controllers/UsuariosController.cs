using System.ComponentModel.DataAnnotations;
using BibliotecaUVSucre.Data;
using BibliotecaUVSucre.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BibliotecaUVSucre.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuariosController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = _userManager.Users.OrderBy(u => u.Nombre).ToList();
            var modelo = new List<UsuarioListItemViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                modelo.Add(new UsuarioListItemViewModel
                {
                    Id = usuario.Id,
                    NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                    Email = usuario.Email ?? string.Empty,
                    Rol = roles.FirstOrDefault() ?? "(sin rol)"
                });
            }

            return View(modelo);
        }

        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(SeedData.Roles);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearUsuarioViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(SeedData.Roles);
                return View(modelo);
            }

            var usuario = new ApplicationUser
            {
                UserName = modelo.Email,
                Email = modelo.Email,
                EmailConfirmed = true,
                Nombre = modelo.Nombre,
                Apellido = modelo.Apellido
            };

            var resultado = await _userManager.CreateAsync(usuario, modelo.Password);
            if (resultado.Succeeded)
            {
                await _userManager.AddToRoleAsync(usuario, modelo.Rol);
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.Roles = new SelectList(SeedData.Roles);
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario != null)
            {
                await _userManager.DeleteAsync(usuario);
            }
            return RedirectToAction(nameof(Index));
        }
    }

    public class UsuarioListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }

    public class CrearUsuarioViewModel
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = "Usuario";
    }
}
