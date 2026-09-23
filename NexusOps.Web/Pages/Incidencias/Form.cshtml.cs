using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Incidencias
{
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
        public IncidenciaFormVm Input { get; set; } = new();

        public int? Id { get; set; }
        public bool EsEdicion => Id.HasValue;
        public string? ActivoTexto { get; set; }

        public List<SelectListItem> Activos { get; set; } = new();
        public List<SelectListItem> Prioridades { get; set; } = new();
        public List<SelectListItem> Usuarios { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id, int? idActivo)
        {
            Id = id;

            if (id.HasValue)
            {
                if (!EsAdminOSupervisor) return Forbid();

                var r = await _api.GetAsync<IncidenciaDto>($"api/incidencias/{id}");
                if (!r.Exito || r.Datos == null)
                {
                    ErrorTemp(r.Error ?? "No se pudo cargar la incidencia.");
                    return RedirectToPage("Index");
                }

                var i = r.Datos;
                if (i.Estado == EstadosIncidencia.Cerrada || i.Estado == EstadosIncidencia.Cancelada)
                {
                    ErrorTemp("No se puede modificar una incidencia cerrada o cancelada.");
                    return RedirectToPage("Detalle", new { id });
                }

                ActivoTexto = $"{i.ActivoCodigo} - {i.ActivoNombre}";
                Input = new IncidenciaFormVm
                {
                    IdActivo = i.IdActivo, Titulo = i.Titulo, Descripcion = i.Descripcion, Prioridad = i.Prioridad,
                    IdUsuarioAsignado = i.IdUsuarioAsignado, Observaciones = i.Observaciones
                };
            }
            else if (idActivo.HasValue)
            {
                Input.IdActivo = idActivo.Value;
            }

            await CargarListasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            Id = id;
            if (id.HasValue && !EsAdminOSupervisor) return Forbid();

            await CargarListasAsync();

            if (id.HasValue)
            {
                // En edición el activo no cambia.
                ModelState.Remove("Input.IdActivo");
            }

            if (!ModelState.IsValid) return Page();

            ApiResultado<IncidenciaDto> r;
            if (id.HasValue)
            {
                r = await _api.PutAsync<IncidenciaDto>($"api/incidencias/{id}", new IncidenciaActualizarDto
                {
                    Titulo = Input.Titulo, Descripcion = Input.Descripcion, Prioridad = Input.Prioridad,
                    IdUsuarioAsignado = Input.IdUsuarioAsignado, Observaciones = Input.Observaciones
                });
            }
            else
            {
                r = await _api.PostAsync<IncidenciaDto>("api/incidencias", new IncidenciaCrearDto
                {
                    IdActivo = Input.IdActivo, Titulo = Input.Titulo, Descripcion = Input.Descripcion,
                    Prioridad = Input.Prioridad, IdUsuarioAsignado = EsAdminOSupervisor ? Input.IdUsuarioAsignado : null
                });
            }

            if (!r.Exito || r.Datos == null)
            {
                MostrarError(r.Error);
                return Page();
            }

            Exito(id.HasValue ? "✓ Incidencia actualizada correctamente." : $"✓ Incidencia {r.Datos.Codigo} registrada correctamente.");
            return RedirectToPage("Detalle", new { id = r.Datos.IdIncidencia });
        }

        private async Task CargarListasAsync()
        {
            Prioridades = CatalogosService.Lista(Models.Prioridades.Todas, UiHelper.Texto);
            if (!EsEdicion) Activos = await _catalogos.ActivosAsync();
            if (EsAdminOSupervisor) Usuarios = await _catalogos.UsuariosAsync();
        }
    }
}
