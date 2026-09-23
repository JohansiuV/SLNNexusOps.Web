using System.ComponentModel.DataAnnotations;

namespace NexusOps.Web.Models
{
    public class TareaDto
    {
        public int IdTarea { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int IdUsuarioAsignado { get; set; }
        public string UsuarioAsignado { get; set; } = string.Empty;
        public int IdUsuarioCreador { get; set; }
        public string UsuarioCreador { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaVencimiento { get; set; }

        // Estado efectivo: "Vencida" si la fecha pasó y no está completada ni cancelada (Regla 4).
        public string Estado { get; set; } = string.Empty;

        // Estado tal como está guardado en la base de datos.
        public string EstadoRegistrado { get; set; } = string.Empty;

        public bool Vencida { get; set; }
        public DateTime? FechaCompletado { get; set; }
    }

    public class TareaGuardarDto
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150, ErrorMessage = "El título no puede superar los 150 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
        public string? Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el responsable.")]
        public int IdUsuarioAsignado { get; set; }

        [Required(ErrorMessage = "La prioridad es obligatoria.")]
        public string Prioridad { get; set; } = Prioridades.Media;

        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; } = DateTime.Today.AddDays(7);
    }
}
