using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Usuarios
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
        public UsuarioGuardarDto Input { get; set; } = new();

        public int? Id { get; set; }
        public bool EsEdicion => Id.HasValue;
        public List<SelectListItem> Roles { get; set; } = new();
        public List<SelectListItem> Personal { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Id = id;

            if (id.HasValue)
            {
                var r = await _api.GetAsync<UsuarioDto>($"api/usuarios/{id}");
                if (!r.Exito || r.Datos == null)
                {
                    ErrorTemp(r.Error ?? "No se pudo cargar el registro.");
                    return RedirectToPage("Index");
                }

                var x = r.Datos;
                Input = new UsuarioGuardarDto { IdRol = x.IdRol, IdPersonal = x.IdPersonal, NombreUsuario = x.NombreUsuario, Activo = x.Activo };
            }

            await CargarListasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            Id = id;
            await CargarListasAsync();

            if (!id.HasValue && string.IsNullOrWhiteSpace(Input.Password))
                ModelState.AddModelError("Input.Password", "La contraseña es obligatoria.");

            if (!ModelState.IsValid) return Page();

            var r = id.HasValue
                ? await _api.PutAsync<UsuarioDto>($"api/usuarios/{id}", Input)
                : await _api.PostAsync<UsuarioDto>("api/usuarios", Input);

            if (!r.Exito)
            {
                MostrarError(r.Error);
                return Page();
            }

            Exito(id.HasValue ? "✓ Usuario actualizado correctamente." : "✓ Usuario registrado correctamente.");
            return RedirectToPage("Index");
        }

        private async Task CargarListasAsync()
        {
            Roles = await _catalogos.RolesAsync();
            Personal = await _catalogos.PersonalAsync();
            await Task.CompletedTask;
        }
    }
}
