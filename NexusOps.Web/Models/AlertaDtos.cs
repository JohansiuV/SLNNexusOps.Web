namespace NexusOps.Web.Models
{
    public class AlertaDto
    {
        public int IdAlerta { get; set; }
        public int? IdUsuario { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string? Entidad { get; set; }
        public int? IdEntidad { get; set; }
        public bool Leida { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class AlertaContadorDto
    {
        public int NoLeidas { get; set; }
        public int Criticas { get; set; }
    }
}
