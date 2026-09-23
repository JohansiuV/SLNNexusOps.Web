using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Activos
{
    public class IndexModel : NxPageModel
    {
        private readonly ApiService _api;
        private readonly CatalogosService _catalogos;

        public IndexModel(ApiService api, CatalogosService catalogos)
        {
            _api = api;
            _catalogos = catalogos;
        }

        public PaginadoDto<ActivoDto> Resultado { get; set; } = new();
        public List<SelectListItem> Areas { get; set; } = new();
        public List<SelectListItem> Tipos { get; set; } = new();
        public List<SelectListItem> Estados { get; set; } = new();

        public string? Buscar { get; set; }
        public int? IdArea { get; set; }
        public int? IdTipoActivo { get; set; }
        public string? Estado { get; set; }

        public async Task OnGetAsync(string? buscar, int? idArea, int? idTipoActivo, string? estado, int pagina = 1)
        {
            Buscar = buscar; IdArea = idArea; IdTipoActivo = idTipoActivo; Estado = estado;

            Areas = await _catalogos.AreasAsync(soloActivas: false);
            Tipos = await _catalogos.TiposActivoAsync(soloActivos: false);
            Estados = CatalogosService.Lista(EstadosActivo.Todos);

            var r = await _api.GetAsync<PaginadoDto<ActivoDto>>(ApiService.Url("api/activos",
                ("buscar", buscar), ("idArea", idArea), ("idTipoActivo", idTipoActivo), ("estado", estado),
                ("pagina", pagina), ("tamano", 10)));

            if (r.Exito && r.Datos != null) Resultado = r.Datos;
            else MostrarError(r.Error);
        }

        public async Task<IActionResult> OnPostBajaAsync(int id)
        {
            var r = await _api.DeleteAsync($"api/activos/{id}");
            if (r.Exito) Exito("El activo fue dado de baja correctamente.");
            else ErrorTemp(r.Error ?? "No se pudo dar de baja el activo.");
            return RedirectToPage();
        }
    }
}
