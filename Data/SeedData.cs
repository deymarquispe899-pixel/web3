using BibliotecaUVSucre.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaUVSucre.Data
{
    // Se ejecuta una vez al iniciar la aplicación (ver Program.cs) para
    // garantizar que existan los 3 roles y al menos un usuario de cada tipo.
    public static class SeedData
    {
        public static readonly string[] Roles = { "Administrador", "Bibliotecario", "Usuario" };

        public static async Task InicializarAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var rol in Roles)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(new IdentityRole(rol));
                }
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await CrearUsuarioSiNoExisteAsync(userManager, "admin@uvsucre.edu.bo", "Admin123!", "Administrador", "Administrador", "Sistema");
            await CrearUsuarioSiNoExisteAsync(userManager, "bibliotecario@uvsucre.edu.bo", "Biblio123!", "Bibliotecario", "Biblioteca", "UVSucre");
            await CrearUsuarioSiNoExisteAsync(userManager, "usuario@uvsucre.edu.bo", "Usuario123!", "Usuario", "Estudiante", "Prueba");

            if (!await context.Libros.AnyAsync())
            {
                context.Libros.AddRange(
                    new Libro { Titulo = "Clean Code", Autor = "Robert C. Martin", Isbn = "9780132350884", Editorial = "Prentice Hall", Categoria = "Programación", Anio = 2008, CantidadTotal = 5, CantidadDisponible = 5 },
                    new Libro { Titulo = "Cien años de soledad", Autor = "Gabriel García Márquez", Isbn = "9780307474728", Editorial = "Sudamericana", Categoria = "Literatura", Anio = 1967, CantidadTotal = 3, CantidadDisponible = 3 },
                    new Libro { Titulo = "Bases de Datos", Autor = "Raghu Ramakrishnan", Isbn = "9780072465631", Editorial = "McGraw-Hill", Categoria = "Base de Datos", Anio = 2003, CantidadTotal = 4, CantidadDisponible = 4 }
                );
                await context.SaveChangesAsync();
            }
        }

        private static async Task CrearUsuarioSiNoExisteAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string rol,
            string nombre,
            string apellido)
        {
            var usuarioExistente = await userManager.FindByEmailAsync(email);
            if (usuarioExistente != null)
            {
                return;
            }

            var usuario = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Nombre = nombre,
                Apellido = apellido
            };

            var resultado = await userManager.CreateAsync(usuario, password);
            if (resultado.Succeeded)
            {
                await userManager.AddToRoleAsync(usuario, rol);
            }
        }
    }
}
