using System.ComponentModel.DataAnnotations;

namespace NexusOps.Web.Models
{
    // Datos del formulario de incidencia (crear / editar).
    public class IncidenciaFormVm
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

        public int? IdUsuarioAsignado { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden superar los 1000 caracteres.")]
        public string? Observaciones { get; set; }
    }
}
