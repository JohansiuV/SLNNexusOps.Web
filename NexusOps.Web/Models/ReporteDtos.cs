namespace NexusOps.Web.Models
{
    public class ReporteDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public DateTime GeneradoEn { get; set; } = DateTime.Now;
    }

    public class CostoDetalleDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string ActivoCodigo { get; set; } = string.Empty;
        public string ActivoNombre { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string TipoMantenimiento { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal CostoServicio { get; set; }
        public decimal CostoRepuestos { get; set; }
        public decimal CostoTotal { get; set; }
    }

    public class CostoItemDto
    {
        public string Etiqueta { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal CostoServicio { get; set; }
        public decimal CostoRepuestos { get; set; }
        public decimal CostoTotal { get; set; }
    }

    public class CostoPeriodoDto
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string Etiqueta { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Costo { get; set; }
        public decimal Acumulado { get; set; }
    }

    public class ReporteCostosDto
    {
        public decimal CostoServicioTotal { get; set; }
        public decimal CostoRepuestosTotal { get; set; }
        public decimal CostoTotal { get; set; }
        public int TotalMantenimientos { get; set; }
        public List<CostoItemDto> PorArea { get; set; } = new();
        public List<CostoItemDto> PorActivo { get; set; } = new();
        public List<CostoPeriodoDto> PorMes { get; set; } = new();
        public List<CostoDetalleDto> Detalle { get; set; } = new();
        public DateTime GeneradoEn { get; set; } = DateTime.Now;
    }
}
