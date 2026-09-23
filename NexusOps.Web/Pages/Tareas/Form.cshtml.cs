using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Tareas
{
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
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
        public TareaGuardarDto Input { get; set; } = new();

        public int? Id { get; set; }
        public bool EsEdicion => Id.HasValue;

        public List<SelectListItem> Usuarios { get; set; } = new();
        public List<SelectListItem> Prioridades { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Id = id;

            if (id.HasValue)
            {
                var r = await _api.GetAsync<TareaDto>($"api/tareas/{id}");
                if (!r.Exito || r.Datos == null)
                {
                    ErrorTemp(r.Error ?? "No se pudo cargar la tarea.");
                    return RedirectToPage("Index");
                }

                var t = r.Datos;
                if (!EstadosTarea.SinCompletar.Contains(t.EstadoRegistrado))
                {
                    ErrorTemp("No se puede modificar una tarea completada o cancelada.");
                    return RedirectToPage("Index");
                }

                Input = new TareaGuardarDto
                {
                    Titulo = t.Titulo, Descripcion = t.Descripcion, IdUsuarioAsignado = t.IdUsuarioAsignado,
                    Prioridad = t.Prioridad, FechaInicio = t.FechaInicio, FechaVencimiento = t.FechaVencimiento
                };
            }

            await CargarListasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            Id = id;
            await CargarListasAsync();

            if (Input.FechaVencimiento.Date < Input.FechaInicio.Date)
                ModelState.AddModelError("Input.FechaVencimiento", "La fecha de vencimiento no puede ser anterior a la de inicio.");

            if (!ModelState.IsValid) return Page();

            var r = id.HasValue
                ? await _api.PutAsync<TareaDto>($"api/tareas/{id}", Input)
                : await _api.PostAsync<TareaDto>("api/tareas", Input);

            if (!r.Exito)
            {
                MostrarError(r.Error);
                return Page();
            }

            Exito(id.HasValue ? "✓ Tarea actualizada correctamente." : "✓ Tarea registrada correctamente.");
            return RedirectToPage("Index");
        }

        private async Task CargarListasAsync()
        {
            Usuarios = await _catalogos.UsuariosAsync();
            Prioridades = CatalogosService.Lista(Models.Prioridades.Todas, UiHelper.Texto);
        }
    }
}
