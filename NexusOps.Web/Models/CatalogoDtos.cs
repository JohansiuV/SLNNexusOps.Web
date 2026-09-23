using System.ComponentModel.DataAnnotations;

namespace NexusOps.Web.Models
{
    public class AreaDto
    {
        public int IdArea { get; set; }

        [Required(ErrorMessage = "El nombre del área es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class TipoActivoDto
    {
        public int IdTipoActivo { get; set; }

        [Required(ErrorMessage = "El nombre del tipo de activo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class PersonalDto
    {
        public int IdPersonal { get; set; }
        public int IdArea { get; set; }
        public string Area { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string Cargo { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class PersonalGuardarDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un área.")]
        public int IdArea { get; set; }

        [Required(ErrorMessage = "El documento es obligatorio.")]
        [StringLength(20, ErrorMessage = "El documento no puede superar los 20 caracteres.")]
        public string Documento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los nombres no pueden superar los 100 caracteres.")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden superar los 100 caracteres.")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
        public string Correo { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El cargo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El cargo no puede superar los 100 caracteres.")]
        public string Cargo { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }

    public class RepuestoDto
    {
        public int IdRepuesto { get; set; }

        [Required(ErrorMessage = "El nombre del repuesto es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        public string? Descripcion { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public int StockMinimo { get; set; }

        [Range(0, 9999999, ErrorMessage = "El precio no puede ser negativo.")]
        public decimal Precio { get; set; }

        public bool Activo { get; set; } = true;

        // Solo lectura: Stock <= StockMinimo.
        public bool StockBajo { get; set; }
    }
}
