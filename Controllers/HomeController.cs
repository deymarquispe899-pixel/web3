using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaUVSucre.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // Punto de entrada tras el login: cada rol ve su propio dashboard (principal).
        public IActionResult Index()
        {
            if (User.IsInRole("Administrador"))
            {
                return View("DashboardAdmin");
            }
            if (User.IsInRole("Bibliotecario"))
            {
                return View("DashboardBibliotecario");
            }
            return View("DashboardUsuario");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
