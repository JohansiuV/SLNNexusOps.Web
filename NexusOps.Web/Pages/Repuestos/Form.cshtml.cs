using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Repuestos
{
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public class FormModel : NxPageModel
    {
        private readonly ApiService _api;
        private readonly CatalogosService _catalogos;

        public FormModel(ApiService api, CatalogosService catalogos)
        {
            _api = api;
            _catalogos = catalogos;
        }

        [BindProperty]
        public RepuestoDto Input { get; set; } = new();

        public int? Id { get; set; }
        public bool EsEdicion => Id.HasValue;


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Id = id;

            if (id.HasValue)
            {
                var r = await _api.GetAsync<RepuestoDto>($"api/repuestos/{id}");
                if (!r.Exito || r.Datos == null)
                {
                    ErrorTemp(r.Error ?? "No se pudo cargar el registro.");
                    return RedirectToPage("Index");
                }

                var x = r.Datos;
                Input = new RepuestoDto { IdRepuesto = x.IdRepuesto, Nombre = x.Nombre, Descripcion = x.Descripcion, Stock = x.Stock, StockMinimo = x.StockMinimo, Precio = x.Precio, Activo = x.Activo };
            }

            await CargarListasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            Id = id;
            await CargarListasAsync();

            if (!ModelState.IsValid) return Page();

            var r = id.HasValue
                ? await _api.PutAsync<RepuestoDto>($"api/repuestos/{id}", Input)
                : await _api.PostAsync<RepuestoDto>("api/repuestos", Input);

            if (!r.Exito)
            {
                MostrarError(r.Error);
                return Page();
            }

            Exito(id.HasValue ? "✓ Repuesto actualizado correctamente." : "✓ Repuesto registrado correctamente.");
            return RedirectToPage("Index");
        }

        private async Task CargarListasAsync()
        {

            await Task.CompletedTask;
        }
    }
}
