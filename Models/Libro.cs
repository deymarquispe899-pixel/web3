using System.ComponentModel.DataAnnotations;

namespace BibliotecaUVSucre.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200)]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(150)]
        public string Autor { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "ISBN")]
        public string? Isbn { get; set; }

        [StringLength(100)]
        public string? Editorial { get; set; }

        [StringLength(80)]
        public string? Categoria { get; set; }

        [Display(Name = "Año de publicación")]
        [Range(1000, 2100)]
        public int? Anio { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa")]
        [Display(Name = "Ejemplares totales")]
        public int CantidadTotal { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Ejemplares disponibles")]
        public int CantidadDisponible { get; set; }

        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}
