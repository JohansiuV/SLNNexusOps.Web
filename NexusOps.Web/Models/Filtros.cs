namespace NexusOps.Web.Models
{
    // Los filtros se reciben por query string (GET) y también se usan para los reportes.

    public class FiltroPaginado
    {
        private int _pagina = 1;
        private int _tamano = 10;

        public int Pagina
        {
            get => _pagina;
            set => _pagina = value < 1 ? 1 : value;
        }

        public int Tamano
        {
            get => _tamano;
            set => _tamano = value < 1 ? 10 : (value > 200 ? 200 : value);
        }

        public string? Buscar { get; set; }
    }

    public class UsuarioFiltro : FiltroPaginado
    {
        public int? IdRol { get; set; }
        public bool? Activo { get; set; }
    }

    public class PersonalFiltro : FiltroPaginado
    {
        public int? IdArea { get; set; }
        public bool? Activo { get; set; }
    }

    public class RepuestoFiltro : FiltroPaginado
    {
        public bool? SoloStockBajo { get; set; }
        public bool? Activo { get; set; }
    }

    public class ActivoFiltro : FiltroPaginado
    {
        public int? IdArea { get; set; }
        public int? IdTipoActivo { get; set; }
        public string? Estado { get; set; }

        // Rango de fecha de adquisición.
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }

    public class IncidenciaFiltro : FiltroPaginado
    {
        public string? Prioridad { get; set; }
        public string? Estado { get; set; }
        public int? IdArea { get; set; }
        public int? IdActivo { get; set; }
        public int? IdUsuarioAsignado { get; set; }
        public bool? SoloAbiertas { get; set; }

        // Rango de fecha de registro.
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }

    public class TareaFiltro : FiltroPaginado
    {
        // Admite "Vencida" (estado calculado).
        public string? Estado { get; set; }
        public string? Prioridad { get; set; }
        public int? IdUsuarioAsignado { get; set; }

        // Rango de fecha de vencimiento.
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }

    public class MantenimientoFiltro : FiltroPaginado
    {
        public string? Tipo { get; set; }
        public string? Estado { get; set; }
        public int? IdTecnico { get; set; }
        public int? IdActivo { get; set; }
        public int? IdArea { get; set; }

        // Rango de fecha programada.
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }

    public class AlertaFiltro : FiltroPaginado
    {
        public bool? SoloNoLeidas { get; set; }
        public string? Nivel { get; set; }
    }

    public class CostoFiltro
    {
        public int? IdArea { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }
}
