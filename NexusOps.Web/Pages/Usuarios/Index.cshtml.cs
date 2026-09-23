using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Usuarios
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

        public List<UsuarioDto> Items { get; set; } = new();
        public PaginacionInfo? Pag { get; set; }
        public List<SelectListItem> Roles { get; set; } = new();
        public List<SelectListItem> EstadosActivo { get; set; } = new() { new("Activo", "true"), new("Inactivo", "false") };
        public string? Buscar { get; set; }
        public int? IdRol { get; set; }
        public string? Activo { get; set; }

        public async Task OnGetAsync(string? buscar, int? idRol, bool? activo, int pagina = 1)
        {
            Buscar = buscar; IdRol = idRol; Activo = activo?.ToString().ToLowerInvariant();
            Roles = await _catalogos.RolesAsync();
            var r = await _api.GetAsync<PaginadoDto<UsuarioDto>>(ApiService.Url("api/usuarios",
                ("buscar", buscar), ("idRol", idRol), ("activo", activo), ("pagina", pagina), ("tamano", 10)));

            if (r.Exito && r.Datos != null)
            {
                Items = r.Datos.Items;
                Pag = new PaginacionInfo(r.Datos.Pagina, r.Datos.TotalPaginas, r.Datos.Total, r.Datos.Tamano);
            }
            else MostrarError(r.Error);
        }

        public async Task<IActionResult> OnPostToggleAsync(int id)
        {
            var r = await _api.DeleteAsync($"api/usuarios/{id}");
            if (r.Exito) Exito("✓ Usuario desactivado correctamente.");
            else ErrorTemp(r.Error ?? "No se pudo desactivar el usuario.");
            return RedirectToPage();
        }
    }
}
