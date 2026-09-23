using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Incidencias
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

        public PaginadoDto<IncidenciaDto> Resultado { get; set; } = new();
        public List<SelectListItem> Areas { get; set; } = new();
        public List<SelectListItem> Prioridades { get; set; } = new();
        public List<SelectListItem> Estados { get; set; } = new();

        public string? Buscar { get; set; }
        public string? Prioridad { get; set; }
        public string? Estado { get; set; }
        public int? IdArea { get; set; }
        public bool SoloAbiertas { get; set; }

        public async Task OnGetAsync(string? buscar, string? prioridad, string? estado, int? idArea, bool soloAbiertas = false, int pagina = 1)
        {
            Buscar = buscar; Prioridad = prioridad; Estado = estado; IdArea = idArea; SoloAbiertas = soloAbiertas;

            Areas = await _catalogos.AreasAsync(soloActivas: false);
            Prioridades = CatalogosService.Lista(Models.Prioridades.Todas, UiHelper.Texto);
            Estados = CatalogosService.Lista(EstadosIncidencia.Todos);

            var r = await _api.GetAsync<PaginadoDto<IncidenciaDto>>(ApiService.Url("api/incidencias",
                ("buscar", buscar), ("prioridad", prioridad), ("estado", estado), ("idArea", idArea),
                ("soloAbiertas", soloAbiertas ? true : null), ("pagina", pagina), ("tamano", 10)));

            if (r.Exito && r.Datos != null) Resultado = r.Datos;
            else MostrarError(r.Error);
        }
    }
}
