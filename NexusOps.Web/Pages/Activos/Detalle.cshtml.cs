using Microsoft.AspNetCore.Mvc;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Activos
{
    public class DetalleModel : NxPageModel
    {
        private readonly ApiService _api;

        public DetalleModel(ApiService api)
        {
            _api = api;
        }

        public ActivoDetalleDto Detalle { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!await CargarAsync(id)) return RedirectToPage("Index");
            return Page();
        }

        public async Task<IActionResult> OnPostEstadoAsync(int id, string estado, string? comentario)
        {
            var r = await _api.PostAsync<ActivoDto>($"api/activos/{id}/estado", new CambiarEstadoDto { Estado = estado, Comentario = comentario });

            if (r.Exito) Exito($"✓ El estado del activo cambió a «{estado}».");
            else ErrorTemp(r.Error ?? "No se pudo cambiar el estado.");

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostBajaAsync(int id)
        {
            var r = await _api.DeleteAsync($"api/activos/{id}");

            if (r.Exito) Exito("✓ El activo fue dado de baja.");
            else ErrorTemp(r.Error ?? "No se pudo dar de baja el activo.");

            return RedirectToPage(new { id });
        }

        private async Task<bool> CargarAsync(int id)
        {
            var r = await _api.GetAsync<ActivoDetalleDto>($"api/activos/{id}");
            if (!r.Exito || r.Datos == null)
            {
                ErrorTemp(r.Error ?? "No se pudo cargar el activo.");
                return false;
            }

            Detalle = r.Datos;
            return true;
        }
    }
}
