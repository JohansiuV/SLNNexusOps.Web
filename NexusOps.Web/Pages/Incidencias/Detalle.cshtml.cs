using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Incidencias
{
    public class DetalleModel : NxPageModel
    {
        private readonly ApiService _api;
        private readonly CatalogosService _catalogos;

        public DetalleModel(ApiService api, CatalogosService catalogos)
        {
            _api = api;
            _catalogos = catalogos;
        }

        public IncidenciaDto Incidencia { get; set; } = new();
        public List<SelectListItem> Usuarios { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var r = await _api.GetAsync<IncidenciaDto>($"api/incidencias/{id}");
            if (!r.Exito || r.Datos == null)
            {
                ErrorTemp(r.Error ?? "No se pudo cargar la incidencia.");
                return RedirectToPage("Index");
            }

            Incidencia = r.Datos;
            if (EsAdminOSupervisor) Usuarios = await _catalogos.UsuariosAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEstadoAsync(int id, string estado, string? comentario, int? idUsuarioAsignado)
        {
            var r = await _api.PostAsync<IncidenciaDto>($"api/incidencias/{id}/estado",
                new CambiarEstadoDto { Estado = estado, Comentario = comentario, IdUsuarioAsignado = idUsuarioAsignado });

            if (r.Exito) Exito($"✓ La incidencia cambió a «{estado}».");
            else ErrorTemp(r.Error ?? "No se pudo cambiar el estado de la incidencia.");

            return RedirectToPage(new { id });
        }
    }
}
