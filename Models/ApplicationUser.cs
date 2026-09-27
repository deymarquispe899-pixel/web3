using Microsoft.AspNetCore.Identity;

namespace BibliotecaUVSucre.Models
{
    // Extiende el usuario por defecto de Identity con datos propios de la biblioteca.
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;

        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}
