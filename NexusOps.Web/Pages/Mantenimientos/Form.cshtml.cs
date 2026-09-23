using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Mantenimientos
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
        public MantenimientoGuardarDto Input { get; set; } = new();

        public int? Id { get; set; }
        public bool EsEdicion => Id.HasValue;

        // Estado del mantenimiento que se edita (para mostrar el contexto).
        public string? Estado { get; set; }
        public string? Codigo { get; set; }
        public string? ActivoTexto { get; set; }
        public string? TecnicoTexto { get; set; }

        // El técnico solo puede registrar costo, observaciones y repuestos.
        public bool SoloTecnico => EsEdicion && EsTecnico;

        public List<SelectListItem> Activos { get; set; } = new();
        public List<SelectListItem> Tecnicos { get; set; } = new();
        public List<SelectListItem> Tipos { get; set; } = new();
        public List<SelectListItem> Repuestos { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id, int? idActivo)
        {
            Id = id;

            if (!id.HasValue && !EsAdminOSupervisor) return Forbid();

            if (id.HasValue)
            {
                var r = await _api.GetAsync<MantenimientoDto>($"api/mantenimientos/{id}");
                if (!r.Exito || r.Datos == null)
                {
                    ErrorTemp(r.Error ?? "No se pudo cargar el mantenimiento.");
                    return RedirectToPage("Index");
                }

                var m = r.Datos;
                if (m.Estado != EstadosMantenimiento.Programado && m.Estado != EstadosMantenimiento.EnProceso)
                {
                    ErrorTemp("No se puede modificar un mantenimiento completado o cancelado.");
                    return RedirectToPage("Detalle", new { id });
                }

                Estado = m.Estado;
                Codigo = m.Codigo;
                ActivoTexto = $"{m.ActivoCodigo} - {m.ActivoNombre}";
                TecnicoTexto = m.Tecnico;

                Input = new MantenimientoGuardarDto
                {
                    IdActivo = m.IdActivo, IdTecnico = m.IdTecnico, TipoMantenimiento = m.TipoMantenimiento,
                    FechaProgramada = m.FechaProgramada, Descripcion = m.Descripcion, Costo = m.Costo,
                    Observaciones = m.Observaciones,
                    Detalles = m.Detalles.Select(d => new DetalleMantenimientoGuardarDto { IdRepuesto = d.IdRepuesto, Cantidad = d.Cantidad }).ToList()
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
            if (!id.HasValue && !EsAdminOSupervisor) return Forbid();

            await CargarListasAsync();

            // Las filas de repuestos sin seleccionar se descartan; el resto se valida aquí.
            foreach (var clave in ModelState.Keys.Where(k => k.StartsWith("Input.Detalles", StringComparison.Ordinal)).ToList())
                ModelState.Remove(clave);

            Input.Detalles = Input.Detalles.Where(d => d.IdRepuesto > 0).ToList();

            if (Input.Detalles.Any(d => d.Cantidad < 1))
                ModelState.AddModelError(string.Empty, "La cantidad de cada repuesto debe ser mayor a cero.");

            if (Input.Detalles.GroupBy(d => d.IdRepuesto).Any(g => g.Count() > 1))
                ModelState.AddModelError(string.Empty, "No repita el mismo repuesto: sume las cantidades en una sola fila.");

            if (!ModelState.IsValid)
            {
                if (SoloTecnico) await RecargarContextoAsync(id!.Value);
                return Page();
            }

            var r = id.HasValue
                ? await _api.PutAsync<MantenimientoDto>($"api/mantenimientos/{id}", Input)
                : await _api.PostAsync<MantenimientoDto>("api/mantenimientos", Input);

            if (!r.Exito || r.Datos == null)
            {
                MostrarError(r.Error);
                if (SoloTecnico) await RecargarContextoAsync(id!.Value);
                return Page();
            }

            Exito(id.HasValue ? "✓ Mantenimiento actualizado correctamente." : $"✓ Mantenimiento {r.Datos.Codigo} programado correctamente.");
            return RedirectToPage("Detalle", new { id = r.Datos.IdMantenimiento });
        }

        private async Task RecargarContextoAsync(int id)
        {
            var r = await _api.GetAsync<MantenimientoDto>($"api/mantenimientos/{id}");
            if (r.Datos == null) return;
            Estado = r.Datos.Estado;
            Codigo = r.Datos.Codigo;
            ActivoTexto = $"{r.Datos.ActivoCodigo} - {r.Datos.ActivoNombre}";
            TecnicoTexto = r.Datos.Tecnico;
        }

        private async Task CargarListasAsync()
        {
            Tipos = CatalogosService.Lista(TiposMantenimiento.Todos);
            Repuestos = await _catalogos.RepuestosAsync();

            if (EsAdminOSupervisor)
            {
                Activos = await _catalogos.ActivosAsync();
                Tecnicos = await _catalogos.UsuariosAsync(RolesSistema.Tecnico);
            }
        }
    }
}
