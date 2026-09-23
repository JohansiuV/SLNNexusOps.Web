using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Personal
{
    [Authorize(Roles = RolesSistema.Administrador)]
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
        public PersonalGuardarDto Input { get; set; } = new();

        public int? Id { get; set; }
        public bool EsEdicion => Id.HasValue;
        public List<SelectListItem> Areas { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Id = id;

            if (id.HasValue)
            {
                var r = await _api.GetAsync<PersonalDto>($"api/personal/{id}");
                if (!r.Exito || r.Datos == null)
                {
                    ErrorTemp(r.Error ?? "No se pudo cargar el registro.");
                    return RedirectToPage("Index");
                }

                var x = r.Datos;
                Input = new PersonalGuardarDto { IdArea = x.IdArea, Documento = x.Documento, Nombres = x.Nombres, Apellidos = x.Apellidos, Correo = x.Correo, Telefono = x.Telefono, Cargo = x.Cargo, Activo = x.Activo };
            }

            await CargarListasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            Id = id;
            await CargarListasAsync();

            if (!ModelState.IsValid) return Page();

            var r = id.HasValue
                ? await _api.PutAsync<PersonalDto>($"api/personal/{id}", Input)
                : await _api.PostAsync<PersonalDto>("api/personal", Input);

            if (!r.Exito)
            {
                MostrarError(r.Error);
                return Page();
            }

            Exito(id.HasValue ? "✓ Personal actualizado correctamente." : "✓ Personal registrado correctamente.");
            return RedirectToPage("Index");
        }

        private async Task CargarListasAsync()
        {
            Areas = await _catalogos.AreasAsync();
            await Task.CompletedTask;
        }
    }
}
