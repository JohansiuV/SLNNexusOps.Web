using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.TiposActivo
{
    [Authorize(Roles = RolesSistema.Administrador)]
    public class IndexModel : NxPageModel
    {
        private readonly ApiService _api;
        private readonly CatalogosService _catalogos;

        public IndexModel(ApiService api, CatalogosService catalogos)
        {
            _api = api;
            _catalogos = catalogos;
        }

        public List<TipoActivoDto> Items { get; set; } = new();
        public PaginacionInfo? Pag { get; set; }
        public string? Buscar { get; set; }

        public async Task OnGetAsync(string? buscar)
        {
            Buscar = buscar;

            var r = await _api.GetAsync<List<TipoActivoDto>>(ApiService.Url("api/tiposactivo", ("buscar", buscar)));

            if (r.Exito && r.Datos != null) Items = r.Datos;
            else MostrarError(r.Error);
        }

        public async Task<IActionResult> OnPostToggleAsync(int id)
        {
            var actual = await _api.GetAsync<TipoActivoDto>($"api/tiposactivo/{id}");
            if (!actual.Exito || actual.Datos == null)
            {
                ErrorTemp(actual.Error ?? "No se pudo obtener el registro.");
                return RedirectToPage();
            }

            var x = actual.Datos;
            var nuevoEstado = !x.Activo;
            var cuerpo = new TipoActivoDto { IdTipoActivo = x.IdTipoActivo, Nombre = x.Nombre, Descripcion = x.Descripcion, Activo = nuevoEstado };

            var r = await _api.PutAsync<TipoActivoDto>($"api/tiposactivo/{id}", cuerpo);
            if (r.Exito) Exito(nuevoEstado ? "✓ Registro activado correctamente." : "✓ Registro desactivado correctamente.");
            else ErrorTemp(r.Error ?? "No se pudo actualizar el registro.");

            return RedirectToPage();
        }
    }
}
