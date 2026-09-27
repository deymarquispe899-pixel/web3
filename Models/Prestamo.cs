using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotecaUVSucre.Models
{
    public enum EstadoPrestamo
    {
        Prestado,
        Devuelto,
        Atrasado
    }

    public class Prestamo
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Libro")]
        public int LibroId { get; set; }

        [ForeignKey(nameof(LibroId))]
        public Libro? Libro { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public string UsuarioId { get; set; } = string.Empty;

        [ForeignKey(nameof(UsuarioId))]
        public ApplicationUser? Usuario { get; set; }

        [Display(Name = "Fecha de préstamo")]
        [DataType(DataType.Date)]
        public DateTime FechaPrestamo { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de devolución esperada")]
        [DataType(DataType.Date)]
        public DateTime FechaDevolucionEsperada { get; set; } = DateTime.Now.AddDays(7);

        [Display(Name = "Fecha de devolución real")]
        [DataType(DataType.Date)]
        public DateTime? FechaDevolucionReal { get; set; }

        [Display(Name = "Estado")]
        public EstadoPrestamo Estado { get; set; } = EstadoPrestamo.Prestado;
    }
}
