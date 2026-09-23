using System.ComponentModel.DataAnnotations;

namespace NexusOps.Web.Models
{
    // Cambio de estado de activos, incidencias, tareas y mantenimientos.
    public class CambiarEstadoDto
    {
        [Required(ErrorMessage = "El nuevo estado es obligatorio.")]
        public string Estado { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
        public string? Comentario { get; set; }

        // Solo para incidencias: permite asignar responsable al mismo tiempo.
        public int? IdUsuarioAsignado { get; set; }
    }
}
