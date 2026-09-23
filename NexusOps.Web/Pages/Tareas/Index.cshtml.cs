using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Tareas
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

        public PaginadoDto<TareaDto> Resultado { get; set; } = new();
        public List<SelectListItem> Estados { get; set; } = new();
        public List<SelectListItem> Prioridades { get; set; } = new();
        public List<SelectListItem> Usuarios { get; set; } = new();

        public string? Buscar { get; set; }
        public string? Estado { get; set; }
        public string? Prioridad { get; set; }
        public int? IdUsuarioAsignado { get; set; }

        public async Task OnGetAsync(string? buscar, string? estado, string? prioridad, int? idUsuarioAsignado, int pagina = 1)
        {
            Buscar = buscar; Estado = estado; Prioridad = prioridad; IdUsuarioAsignado = idUsuarioAsignado;

            Estados = CatalogosService.Lista(EstadosTarea.Todos);
            Prioridades = CatalogosService.Lista(Models.Prioridades.Todas, UiHelper.Texto);
            if (EsAdminOSupervisor) Usuarios = await _catalogos.UsuariosAsync();

            var r = await _api.GetAsync<PaginadoDto<TareaDto>>(ApiService.Url("api/tareas",
                ("buscar", buscar), ("estado", estado), ("prioridad", prioridad), ("idUsuarioAsignado", idUsuarioAsignado),
                ("pagina", pagina), ("tamano", 10)));

            if (r.Exito && r.Datos != null) Resultado = r.Datos;
            else MostrarError(r.Error);
        }

        public async Task<IActionResult> OnPostEstadoAsync(int id, string estado, string? comentario)
        {
            var r = await _api.PostAsync<TareaDto>($"api/tareas/{id}/estado", new CambiarEstadoDto { Estado = estado, Comentario = comentario });

            if (r.Exito) Exito($"✓ La tarea cambió a «{estado}».");
            else ErrorTemp(r.Error ?? "No se pudo cambiar el estado de la tarea.");

            return RedirectToPage();
        }
    }
}
