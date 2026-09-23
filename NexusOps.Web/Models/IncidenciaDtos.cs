using System.ComponentModel.DataAnnotations;

namespace NexusOps.Web.Models
{
    public class IncidenciaDto
    {
        public int IdIncidencia { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public int IdActivo { get; set; }
        public string ActivoCodigo { get; set; } = string.Empty;
        public string ActivoNombre { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public int IdUsuarioReporta { get; set; }
        public string UsuarioReporta { get; set; } = string.Empty;
        public int? IdUsuarioAsignado { get; set; }
        public string? UsuarioAsignado { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string? Observaciones { get; set; }

        // Solo se completa en GET api/incidencias/{id}.
        public List<HistorialDto> Historial { get; set; } = new();
    }

    public class IncidenciaCrearDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el activo afectado.")]
        public int IdActivo { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150, ErrorMessage = "El título no puede superar los 150 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La prioridad es obligatoria.")]
        public string Prioridad { get; set; } = Prioridades.Media;

        // Opcional; solo Administrador/Supervisor pueden asignar al registrar.
        public int? IdUsuarioAsignado { get; set; }
    }

    public class IncidenciaActualizarDto
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150, ErrorMessage = "El título no puede superar los 150 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La prioridad es obligatoria.")]
        public string Prioridad { get; set; } = Prioridades.Media;

        public int? IdUsuarioAsignado { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden superar los 1000 caracteres.")]
        public string? Observaciones { get; set; }
    }
}
