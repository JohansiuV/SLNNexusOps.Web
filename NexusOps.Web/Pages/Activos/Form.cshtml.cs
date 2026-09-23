using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Activos
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
        public ActivoGuardarDto Input { get; set; } = new();

        public int? Id { get; set; }
        public bool EsEdicion => Id.HasValue;

        public List<SelectListItem> Areas { get; set; } = new();
        public List<SelectListItem> Tipos { get; set; } = new();
        public List<SelectListItem> Estados { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Id = id;

            if (id.HasValue)
            {
                var r = await _api.GetAsync<ActivoDetalleDto>($"api/activos/{id}");
                if (!r.Exito || r.Datos == null)
                {
                    ErrorTemp(r.Error ?? "No se pudo cargar el activo.");
                    return RedirectToPage("Index");
                }

                var a = r.Datos.Activo;
                if (a.Estado == EstadosActivo.DadoDeBaja)
                {
                    ErrorTemp("Un activo dado de baja no puede modificarse.");
                    return RedirectToPage("Detalle", new { id });
                }

                Input = new ActivoGuardarDto
                {
                    IdTipoActivo = a.IdTipoActivo, IdArea = a.IdArea, Codigo = a.Codigo, Nombre = a.Nombre,
                    Marca = a.Marca, Modelo = a.Modelo, NumeroSerie = a.NumeroSerie,
                    FechaAdquisicion = a.FechaAdquisicion, ValorAdquisicion = a.ValorAdquisicion,
                    Estado = a.Estado, Observaciones = a.Observaciones
                };
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
                ? await _api.PutAsync<ActivoDto>($"api/activos/{id}", Input)
                : await _api.PostAsync<ActivoDto>("api/activos", Input);

            if (!r.Exito || r.Datos == null)
            {
                MostrarError(r.Error);
                return Page();
            }

            Exito(id.HasValue ? "✓ Activo actualizado correctamente." : "✓ Activo registrado correctamente.");
            return RedirectToPage("Detalle", new { id = r.Datos.IdActivo });
        }

        private async Task CargarListasAsync()
        {
            Areas = await _catalogos.AreasAsync();
            Tipos = await _catalogos.TiposActivoAsync();
            var estados = EsEdicion ? EstadosActivo.Todos : EstadosActivo.Todos.Where(e => e != EstadosActivo.DadoDeBaja).ToArray();
            Estados = CatalogosService.Lista(estados);
        }
    }
}
