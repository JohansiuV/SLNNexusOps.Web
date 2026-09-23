using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Dashboard
{
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public class IndexModel : NxPageModel
    {
        private readonly DashboardService _servicio;

        public IndexModel(DashboardService servicio)
        {
            _servicio = servicio;
        }

        public DashboardResumenDto? Resumen { get; set; }

        public string? MensajeError { get; set; }

        // Datos de los gráficos, serializados para dashboard.js (System.Text.Json escapa <, > y &).
        public string DatosJson { get; set; } = "{}";

        public async Task OnGetAsync()
        {
            var resultado = await _servicio.ObtenerResumenAsync();

            if (!resultado.Exito || resultado.Datos == null)
            {
                MensajeError = resultado.Error;
                return;
            }

            Resumen = resultado.Datos;

            DatosJson = JsonSerializer.Serialize(new
            {
                Resumen.IncidenciasPorPrioridad,
                Resumen.IncidenciasPorArea,
                Resumen.ActivosPorEstado,
                Resumen.MantenimientosPorMes,
                Resumen.TareasPorEstado,
                Resumen.CostosPorMes
            }, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }
    }
}
