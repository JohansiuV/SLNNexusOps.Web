using System.ComponentModel.DataAnnotations;

namespace NexusOps.Web.Models
{
    public class DetalleMantenimientoDto
    {
        public int IdRepuesto { get; set; }
        public string Repuesto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class MantenimientoDto
    {
        public int IdMantenimiento { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public int IdActivo { get; set; }
        public string ActivoCodigo { get; set; } = string.Empty;
        public string ActivoNombre { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public int IdTecnico { get; set; }
        public string Tecnico { get; set; } = string.Empty;
        public string TipoMantenimiento { get; set; } = string.Empty;
        public DateTime FechaProgramada { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Descripcion { get; set; } = string.Empty;

        // Costo de servicio (mano de obra).
        public decimal Costo { get; set; }
        public decimal CostoRepuestos { get; set; }
        public decimal CostoTotal { get; set; }

        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public List<DetalleMantenimientoDto> Detalles { get; set; } = new();
        public List<HistorialDto> Historial { get; set; } = new();
    }

    public class DetalleMantenimientoGuardarDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un repuesto.")]
        public int IdRepuesto { get; set; }

        [Range(1, 10000, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; } = 1;
    }

    public class MantenimientoGuardarDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el activo.")]
        public int IdActivo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el técnico responsable.")]
        public int IdTecnico { get; set; }

        [Required(ErrorMessage = "El tipo de mantenimiento es obligatorio.")]
        public string TipoMantenimiento { get; set; } = TiposMantenimiento.Preventivo;

        [DataType(DataType.Date)]
        public DateTime FechaProgramada { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        [Range(0, 999999999, ErrorMessage = "El costo no puede ser negativo.")]
        public decimal Costo { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden superar los 1000 caracteres.")]
        public string? Observaciones { get; set; }

        public List<DetalleMantenimientoGuardarDto> Detalles { get; set; } = new();
    }
}
