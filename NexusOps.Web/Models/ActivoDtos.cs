using System.ComponentModel.DataAnnotations;

namespace NexusOps.Web.Models
{
    public class ActivoDto
    {
        public int IdActivo { get; set; }
        public int IdTipoActivo { get; set; }
        public string TipoActivo { get; set; } = string.Empty;
        public int IdArea { get; set; }
        public string Area { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? NumeroSerie { get; set; }
        public DateTime FechaAdquisicion { get; set; }
        public decimal ValorAdquisicion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class ActivoGuardarDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el tipo de activo.")]
        public int IdTipoActivo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el área.")]
        public int IdArea { get; set; }

        [Required(ErrorMessage = "El código del activo es obligatorio.")]
        [StringLength(30, ErrorMessage = "El código no puede superar los 30 caracteres.")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del activo es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(80, ErrorMessage = "La marca no puede superar los 80 caracteres.")]
        public string? Marca { get; set; }

        [StringLength(80, ErrorMessage = "El modelo no puede superar los 80 caracteres.")]
        public string? Modelo { get; set; }

        [StringLength(80, ErrorMessage = "El número de serie no puede superar los 80 caracteres.")]
        public string? NumeroSerie { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaAdquisicion { get; set; } = DateTime.Today;

        [Range(0, 999999999, ErrorMessage = "El valor de adquisición no puede ser negativo.")]
        public decimal ValorAdquisicion { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public string Estado { get; set; } = EstadosActivo.Operativo;

        [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }

        // Motivo del cambio de estado (se guarda en el historial).
        [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
        public string? Comentario { get; set; }
    }

    // Ficha detallada del activo.
    public class ActivoDetalleDto
    {
        public ActivoDto Activo { get; set; } = new();
        public List<HistorialDto> Historial { get; set; } = new();
        public List<IncidenciaDto> Incidencias { get; set; } = new();
        public List<MantenimientoDto> Mantenimientos { get; set; } = new();
    }
}
