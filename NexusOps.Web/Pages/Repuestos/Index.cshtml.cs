using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Repuestos
{
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public class IndexModel : NxPageModel
    {
        private readonly ApiService _api;
        private readonly CatalogosService _catalogos;

        public IndexModel(ApiService api, CatalogosService catalogos)
        {
            _api = api;
            _catalogos = catalogos;
        }

        public List<RepuestoDto> Items { get; set; } = new();
        public PaginacionInfo? Pag { get; set; }
        public string? Buscar { get; set; }
        public bool SoloStockBajo { get; set; }

        public async Task OnGetAsync(string? buscar, bool soloStockBajo = false, int pagina = 1)
        {
            Buscar = buscar; SoloStockBajo = soloStockBajo;

            var r = await _api.GetAsync<PaginadoDto<RepuestoDto>>(ApiService.Url("api/repuestos",
                ("buscar", buscar), ("soloStockBajo", soloStockBajo ? true : null), ("pagina", pagina), ("tamano", 10)));

            if (r.Exito && r.Datos != null)
            {
                Items = r.Datos.Items;
                Pag = new PaginacionInfo(r.Datos.Pagina, r.Datos.TotalPaginas, r.Datos.Total, r.Datos.Tamano);
            }
            else MostrarError(r.Error);
        }

        public async Task<IActionResult> OnPostToggleAsync(int id)
        {
            var actual = await _api.GetAsync<RepuestoDto>($"api/repuestos/{id}");
            if (!actual.Exito || actual.Datos == null)
            {
                ErrorTemp(actual.Error ?? "No se pudo obtener el registro.");
                return RedirectToPage();
            }

            var x = actual.Datos;
            var nuevoEstado = !x.Activo;
            var cuerpo = new RepuestoDto { IdRepuesto = x.IdRepuesto, Nombre = x.Nombre, Descripcion = x.Descripcion, Stock = x.Stock, StockMinimo = x.StockMinimo, Precio = x.Precio, Activo = nuevoEstado };

            var r = await _api.PutAsync<RepuestoDto>($"api/repuestos/{id}", cuerpo);
            if (r.Exito) Exito(nuevoEstado ? "✓ Registro activado correctamente." : "✓ Registro desactivado correctamente.");
            else ErrorTemp(r.Error ?? "No se pudo actualizar el registro.");

            return RedirectToPage();
        }
    }
}
