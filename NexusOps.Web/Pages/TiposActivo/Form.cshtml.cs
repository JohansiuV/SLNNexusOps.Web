using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.TiposActivo
{
    [Authorize(Roles = RolesSistema.Administrador)]
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
        public TipoActivoDto Input { get; set; } = new();

        public int? Id { get; set; }
        public bool EsEdicion => Id.HasValue;


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Id = id;

            if (id.HasValue)
            {
                var r = await _api.GetAsync<TipoActivoDto>($"api/tiposactivo/{id}");
                if (!r.Exito || r.Datos == null)
                {
                    ErrorTemp(r.Error ?? "No se pudo cargar el registro.");
                    return RedirectToPage("Index");
                }

                var x = r.Datos;
                Input = new TipoActivoDto { IdTipoActivo = x.IdTipoActivo, Nombre = x.Nombre, Descripcion = x.Descripcion, Activo = x.Activo };
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
                ? await _api.PutAsync<TipoActivoDto>($"api/tiposactivo/{id}", Input)
                : await _api.PostAsync<TipoActivoDto>("api/tiposactivo", Input);

            if (!r.Exito)
            {
                MostrarError(r.Error);
                return Page();
            }

            Exito(id.HasValue ? "✓ Tipo de activo actualizado correctamente." : "✓ Tipo de activo registrado correctamente.");
            return RedirectToPage("Index");
        }

        private async Task CargarListasAsync()
        {

            await Task.CompletedTask;
        }
    }
}
