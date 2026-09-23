namespace NexusOps.Web.Models
{
    public class DashboardTarjetasDto
    {
        public int TotalActivos { get; set; }
        public int Operativos { get; set; }
        public int EnMantenimiento { get; set; }
        public int Incidencias { get; set; }          // abiertas
        public int TareasPendientes { get; set; }
        public int Mantenimientos { get; set; }       // programados + en proceso
    }

    public class IndicadorDto
    {
        // Critica | Advertencia | Informativa
        public string Nivel { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public int Cantidad { get; set; }

        // Ruta de la aplicación web donde se ve el detalle.
        public string? Enlace { get; set; }
    }

    public class MantenimientoPendienteDto
    {
        public int IdMantenimiento { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Activo { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Tecnico { get; set; } = string.Empty;
        public string TipoMantenimiento { get; set; } = string.Empty;
        public DateTime FechaProgramada { get; set; }
        public int DiasRestantes { get; set; }
        public string Situacion { get; set; } = string.Empty;
    }

    public class DashboardResumenDto
    {
        public DashboardTarjetasDto Tarjetas { get; set; } = new();
        public List<IndicadorDto> Indicadores { get; set; } = new();
        public List<EtiquetaValorDto> IncidenciasPorPrioridad { get; set; } = new();
        public List<EtiquetaValorDto> IncidenciasPorArea { get; set; } = new();
        public List<EtiquetaValorDto> ActivosPorEstado { get; set; } = new();
        public List<EtiquetaValorDto> MantenimientosPorMes { get; set; } = new();
        public List<EtiquetaValorDto> TareasPorEstado { get; set; } = new();
        public List<EtiquetaValorDto> CostosPorMes { get; set; } = new();
        public List<MantenimientoPendienteDto> ProximosMantenimientos { get; set; } = new();
        public List<AlertaDto> AlertasRecientes { get; set; } = new();
        public DateTime GeneradoEn { get; set; } = DateTime.Now;
    }
}
