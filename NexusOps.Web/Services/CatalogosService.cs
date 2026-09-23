using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Models;

namespace NexusOps.Web.Services
{
    // Listas desplegables (áreas, tipos, usuarios, activos...) obtenidas desde la API.
    public class CatalogosService
    {
        private readonly ApiService _api;

        public CatalogosService(ApiService api)
        {
            _api = api;
        }

        public async Task<List<SelectListItem>> AreasAsync(bool soloActivas = true)
        {
            var r = await _api.GetAsync<List<AreaDto>>(ApiService.Url("api/areas", ("soloActivas", soloActivas)));
            return (r.Datos ?? new()).Select(a => new SelectListItem(a.Nombre, a.IdArea.ToString())).ToList();
        }

        public async Task<List<SelectListItem>> TiposActivoAsync(bool soloActivos = true)
        {
            var r = await _api.GetAsync<List<TipoActivoDto>>(ApiService.Url("api/tiposactivo", ("soloActivos", soloActivos)));
            return (r.Datos ?? new()).Select(t => new SelectListItem(t.Nombre, t.IdTipoActivo.ToString())).ToList();
        }

        public async Task<List<SelectListItem>> RolesAsync()
        {
            var r = await _api.GetAsync<List<RolDto>>("api/roles");
            return (r.Datos ?? new()).Where(x => x.Activo).Select(x => new SelectListItem(x.Nombre, x.IdRol.ToString())).ToList();
        }

        public async Task<List<SelectListItem>> PersonalAsync()
        {
            var r = await _api.GetAsync<PaginadoDto<PersonalDto>>(ApiService.Url("api/personal", ("activo", true), ("tamano", 200)));
            return (r.Datos?.Items ?? new())
                .Select(p => new SelectListItem($"{p.NombreCompleto} - {p.Cargo}", p.IdPersonal.ToString())).ToList();
        }

        // Usuarios activos (Administrador/Supervisor): para asignar incidencias, tareas y mantenimientos.
        public async Task<List<UsuarioSimpleDto>> UsuariosAsignablesAsync()
        {
            var r = await _api.GetAsync<List<UsuarioSimpleDto>>("api/usuarios/asignables");
            return r.Datos ?? new();
        }

        public async Task<List<SelectListItem>> UsuariosAsync(string? soloRol = null)
        {
            var lista = await UsuariosAsignablesAsync();
            return lista.Where(u => soloRol == null || u.Rol == soloRol)
                        .Select(u => new SelectListItem($"{u.NombreCompleto} ({u.Rol})", u.IdUsuario.ToString())).ToList();
        }

        // Activos disponibles para reportar incidencias o programar mantenimientos (excluye dados de baja).
        public async Task<List<SelectListItem>> ActivosAsync()
        {
            var r = await _api.GetAsync<PaginadoDto<ActivoDto>>(ApiService.Url("api/activos", ("tamano", 200)));
            return (r.Datos?.Items ?? new())
                .Where(a => a.Estado != EstadosActivo.DadoDeBaja)
                .Select(a => new SelectListItem($"{a.Codigo} - {a.Nombre}", a.IdActivo.ToString())).ToList();
        }

        public async Task<List<SelectListItem>> RepuestosAsync()
        {
            var r = await _api.GetAsync<PaginadoDto<RepuestoDto>>(ApiService.Url("api/repuestos", ("activo", true), ("tamano", 200)));
            return (r.Datos?.Items ?? new())
                .Select(x => new SelectListItem($"{x.Nombre} (stock {x.Stock})", x.IdRepuesto.ToString())).ToList();
        }

        public static List<SelectListItem> Lista(IEnumerable<string> valores, Func<string, string>? texto = null) =>
            valores.Select(v => new SelectListItem(texto?.Invoke(v) ?? v, v)).ToList();
    }
}
