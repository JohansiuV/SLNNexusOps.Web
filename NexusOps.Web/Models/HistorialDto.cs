namespace NexusOps.Web.Models
{
    public class HistorialDto
    {
        public int IdHistorial { get; set; }
        public string Entidad { get; set; } = string.Empty;
        public int IdEntidad { get; set; }
        public int? IdActivo { get; set; }
        public string? EstadoAnterior { get; set; }
        public string EstadoNuevo { get; set; } = string.Empty;
        public string? Comentario { get; set; }
        public string? Usuario { get; set; }
        public DateTime Fecha { get; set; }
    }
}
