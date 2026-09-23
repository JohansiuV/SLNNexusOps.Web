using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Mantenimientos
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

        public PaginadoDto<MantenimientoDto> Resultado { get; set; } = new();
        public List<SelectListItem> Tipos { get; set; } = new();
        public List<SelectListItem> Estados { get; set; } = new();
        public List<SelectListItem> Tecnicos { get; set; } = new();

        public string? Buscar { get; set; }
        public string? Tipo { get; set; }
        public string? Estado { get; set; }
        public int? IdTecnico { get; set; }

        public async Task OnGetAsync(string? buscar, string? tipo, string? estado, int? idTecnico, int pagina = 1)
        {
            Buscar = buscar; Tipo = tipo; Estado = estado; IdTecnico = idTecnico;

            Tipos = CatalogosService.Lista(TiposMantenimiento.Todos);
            Estados = CatalogosService.Lista(EstadosMantenimiento.Todos);
            if (EsAdminOSupervisor) Tecnicos = await _catalogos.UsuariosAsync(RolesSistema.Tecnico);

            var r = await _api.GetAsync<PaginadoDto<MantenimientoDto>>(ApiService.Url("api/mantenimientos",
                ("buscar", buscar), ("tipo", tipo), ("estado", estado), ("idTecnico", idTecnico),
                ("pagina", pagina), ("tamano", 10)));

            if (r.Exito && r.Datos != null) Resultado = r.Datos;
            else MostrarError(r.Error);
        }
    }
}
