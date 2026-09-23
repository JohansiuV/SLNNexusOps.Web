using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Reportes
{
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public class IndexModel : NxPageModel
    {
        public static readonly string[] TiposReporte = { "activos", "incidencias", "mantenimientos", "costos" };

        private readonly ApiService _api;
        private readonly CatalogosService _catalogos;

        public IndexModel(ApiService api, CatalogosService catalogos)
        {
            _api = api;
            _catalogos = catalogos;
        }

        // Filtros
        public string Tipo { get; set; } = "activos";
        public int? IdArea { get; set; }
        public string? Estado { get; set; }
        public int? IdTipoActivo { get; set; }
        public string? Prioridad { get; set; }
        public string? TipoMant { get; set; }
        public int? IdTecnico { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }

        // Resultados (solo se completa el del tipo seleccionado)
        public ReporteDto<ActivoDto>? Activos { get; set; }
        public ReporteDto<IncidenciaDto>? Incidencias { get; set; }
        public ReporteDto<MantenimientoDto>? Mantenimientos { get; set; }
        public ReporteCostosDto? Costos { get; set; }

        // Listas desplegables
        public List<SelectListItem> Areas { get; set; } = new();
        public List<SelectListItem> TiposActivo { get; set; } = new();
        public List<SelectListItem> Estados { get; set; } = new();
        public List<SelectListItem> Prioridades { get; set; } = new();
        public List<SelectListItem> TiposMantenimiento { get; set; } = new();
        public List<SelectListItem> Tecnicos { get; set; } = new();

        public async Task OnGetAsync(string? tipo, int? idArea, string? estado, int? idTipoActivo, string? prioridad,
                                     string? tipoMant, int? idTecnico, DateTime? desde, DateTime? hasta)
        {
            Asignar(tipo, idArea, estado, idTipoActivo, prioridad, tipoMant, idTecnico, desde, hasta);
            await CargarListasAsync();
            if (!await CargarReporteAsync()) MostrarError(ViewData["ErrorReporte"] as string);
        }

        // Exporta el reporte actual a CSV (abre en Excel).
        public async Task<IActionResult> OnGetCsvAsync(string? tipo, int? idArea, string? estado, int? idTipoActivo, string? prioridad,
                                                       string? tipoMant, int? idTecnico, DateTime? desde, DateTime? hasta)
        {
            Asignar(tipo, idArea, estado, idTipoActivo, prioridad, tipoMant, idTecnico, desde, hasta);

            if (!await CargarReporteAsync())
            {
                ErrorTemp(ViewData["ErrorReporte"] as string ?? "No se pudo generar el reporte.");
                return RedirectToPage(new { tipo = Tipo });
            }

            var (cabeceras, filas) = ConstruirTabla();
            var bytes = CsvHelper.Generar(cabeceras, filas);
            return File(bytes, "text/csv; charset=utf-8", $"reporte-{Tipo}-{DateTime.Now:yyyyMMdd-HHmm}.csv");
        }

        private void Asignar(string? tipo, int? idArea, string? estado, int? idTipoActivo, string? prioridad,
                             string? tipoMant, int? idTecnico, DateTime? desde, DateTime? hasta)
        {
            Tipo = tipo != null && TiposReporte.Contains(tipo) ? tipo : "activos";
            IdArea = idArea; Estado = estado; IdTipoActivo = idTipoActivo; Prioridad = prioridad;
            TipoMant = tipoMant; IdTecnico = idTecnico; Desde = desde; Hasta = hasta;
        }

        private async Task CargarListasAsync()
        {
            Areas = await _catalogos.AreasAsync(soloActivas: false);
            Prioridades = CatalogosService.Lista(Models.Prioridades.Todas, UiHelper.Texto);
            TiposMantenimiento = CatalogosService.Lista(Models.TiposMantenimiento.Todos);

            switch (Tipo)
            {
                case "activos":
                    TiposActivo = await _catalogos.TiposActivoAsync(soloActivos: false);
                    Estados = CatalogosService.Lista(EstadosActivo.Todos);
                    break;
                case "incidencias":
                    Estados = CatalogosService.Lista(EstadosIncidencia.Todos);
                    break;
                case "mantenimientos":
                    Estados = CatalogosService.Lista(EstadosMantenimiento.Todos);
                    Tecnicos = await _catalogos.UsuariosAsync(RolesSistema.Tecnico);
                    break;
            }
        }

        private async Task<bool> CargarReporteAsync()
        {
            string? error = null;

            switch (Tipo)
            {
                case "activos":
                {
                    var r = await _api.GetAsync<ReporteDto<ActivoDto>>(ApiService.Url("api/reportes/activos",
                        ("idArea", IdArea), ("estado", Estado), ("idTipoActivo", IdTipoActivo), ("desde", Desde), ("hasta", Hasta)));
                    Activos = r.Datos; error = r.Exito ? null : r.Error;
                    break;
                }
                case "incidencias":
                {
                    var r = await _api.GetAsync<ReporteDto<IncidenciaDto>>(ApiService.Url("api/reportes/incidencias",
                        ("prioridad", Prioridad), ("estado", Estado), ("idArea", IdArea), ("desde", Desde), ("hasta", Hasta)));
                    Incidencias = r.Datos; error = r.Exito ? null : r.Error;
                    break;
                }
                case "mantenimientos":
                {
                    var r = await _api.GetAsync<ReporteDto<MantenimientoDto>>(ApiService.Url("api/reportes/mantenimientos",
                        ("tipo", TipoMant), ("estado", Estado), ("idTecnico", IdTecnico), ("idArea", IdArea), ("desde", Desde), ("hasta", Hasta)));
                    Mantenimientos = r.Datos; error = r.Exito ? null : r.Error;
                    break;
                }
                default:
                {
                    var r = await _api.GetAsync<ReporteCostosDto>(ApiService.Url("api/reportes/costos",
                        ("idArea", IdArea), ("desde", Desde), ("hasta", Hasta)));
                    Costos = r.Datos; error = r.Exito ? null : r.Error;
                    break;
                }
            }

            if (error != null) ViewData["ErrorReporte"] = error;
            return error == null;
        }

        private (string[] Cabeceras, List<object?[]> Filas) ConstruirTabla()
        {
            switch (Tipo)
            {
                case "activos":
                    return (new[] { "Código", "Nombre", "Tipo", "Área", "Marca", "Modelo", "N° serie", "Fecha adquisición", "Valor (S/)", "Estado" },
                        (Activos?.Items ?? new()).Select(a => new object?[] { a.Codigo, a.Nombre, a.TipoActivo, a.Area, a.Marca, a.Modelo, a.NumeroSerie, a.FechaAdquisicion, a.ValorAdquisicion, a.Estado }).ToList());

                case "incidencias":
                    return (new[] { "Código", "Título", "Activo", "Área", "Prioridad", "Estado", "Reportada por", "Asignada a", "Registro", "Resolución" },
                        (Incidencias?.Items ?? new()).Select(i => new object?[] { i.Codigo, i.Titulo, i.ActivoCodigo, i.Area, UiHelper.Texto(i.Prioridad), i.Estado, i.UsuarioReporta, i.UsuarioAsignado, i.FechaRegistro, i.FechaResolucion }).ToList());

                case "mantenimientos":
                    return (new[] { "Código", "Activo", "Área", "Tipo", "Técnico", "Programado", "Estado", "Costo servicio (S/)", "Costo repuestos (S/)", "Costo total (S/)" },
                        (Mantenimientos?.Items ?? new()).Select(m => new object?[] { m.Codigo, m.ActivoCodigo, m.Area, m.TipoMantenimiento, m.Tecnico, m.FechaProgramada, m.Estado, m.Costo, m.CostoRepuestos, m.CostoTotal }).ToList());

                default:
                    return (new[] { "Código", "Activo", "Área", "Tipo", "Fecha", "Costo servicio (S/)", "Costo repuestos (S/)", "Costo total (S/)" },
                        (Costos?.Detalle ?? new()).Select(c => new object?[] { c.Codigo, c.ActivoCodigo, c.Area, c.TipoMantenimiento, c.Fecha, c.CostoServicio, c.CostoRepuestos, c.CostoTotal }).ToList());
            }
        }
    }
}
