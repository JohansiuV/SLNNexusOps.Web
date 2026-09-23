using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Personal
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

        public List<PersonalDto> Items { get; set; } = new();
        public PaginacionInfo? Pag { get; set; }
        public List<SelectListItem> Areas { get; set; } = new();
        public List<SelectListItem> EstadosActivo { get; set; } = new() { new("Activo", "true"), new("Inactivo", "false") };
        public string? Buscar { get; set; }
        public int? IdArea { get; set; }
        public string? Activo { get; set; }

        public async Task OnGetAsync(string? buscar, int? idArea, bool? activo, int pagina = 1)
        {
            Buscar = buscar; IdArea = idArea; Activo = activo?.ToString().ToLowerInvariant();
            Areas = await _catalogos.AreasAsync(soloActivas: false);
            var r = await _api.GetAsync<PaginadoDto<PersonalDto>>(ApiService.Url("api/personal",
                ("buscar", buscar), ("idArea", idArea), ("activo", activo), ("pagina", pagina), ("tamano", 10)));

            if (r.Exito && r.Datos != null)
            {
                Items = r.Datos.Items;
                Pag = new PaginacionInfo(r.Datos.Pagina, r.Datos.TotalPaginas, r.Datos.Total, r.Datos.Tamano);
            }
            else MostrarError(r.Error);
        }

        public async Task<IActionResult> OnPostToggleAsync(int id)
        {
            var actual = await _api.GetAsync<PersonalDto>($"api/personal/{id}");
            if (!actual.Exito || actual.Datos == null)
            {
                ErrorTemp(actual.Error ?? "No se pudo obtener el registro.");
                return RedirectToPage();
            }

            var x = actual.Datos;
            var nuevoEstado = !x.Activo;
            var cuerpo = new PersonalGuardarDto { IdArea = x.IdArea, Documento = x.Documento, Nombres = x.Nombres, Apellidos = x.Apellidos, Correo = x.Correo, Telefono = x.Telefono, Cargo = x.Cargo, Activo = nuevoEstado };

            var r = await _api.PutAsync<PersonalDto>($"api/personal/{id}", cuerpo);
            if (r.Exito) Exito(nuevoEstado ? "✓ Registro activado correctamente." : "✓ Registro desactivado correctamente.");
            else ErrorTemp(r.Error ?? "No se pudo actualizar el registro.");

            return RedirectToPage();
        }
    }
}
