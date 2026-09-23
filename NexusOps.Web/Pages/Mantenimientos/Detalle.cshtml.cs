using Microsoft.AspNetCore.Mvc;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Mantenimientos
{
    public class DetalleModel : NxPageModel
    {
        private readonly ApiService _api;

        public DetalleModel(ApiService api)
        {
            _api = api;
        }

        public MantenimientoDto Mantenimiento { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var r = await _api.GetAsync<MantenimientoDto>($"api/mantenimientos/{id}");
            if (!r.Exito || r.Datos == null)
            {
                ErrorTemp(r.Error ?? "No se pudo cargar el mantenimiento.");
                return RedirectToPage("Index");
            }

            Mantenimiento = r.Datos;
            return Page();
        }

        public async Task<IActionResult> OnPostEstadoAsync(int id, string estado, string? comentario)
        {
            var r = await _api.PostAsync<MantenimientoDto>($"api/mantenimientos/{id}/estado",
                new CambiarEstadoDto { Estado = estado, Comentario = comentario });

            if (r.Exito) Exito($"✓ El mantenimiento cambió a «{estado}».");
            else ErrorTemp(r.Error ?? "No se pudo cambiar el estado del mantenimiento.");

            return RedirectToPage(new { id });
        }
    }
}
