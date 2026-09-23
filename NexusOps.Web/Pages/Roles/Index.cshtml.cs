using Microsoft.AspNetCore.Authorization;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Roles
{
    [Authorize(Roles = RolesSistema.Administrador)]
    public class IndexModel : NxPageModel
    {
        private readonly ApiService _api;

        public IndexModel(ApiService api)
        {
            _api = api;
        }

        public List<RolDto> Roles { get; set; } = new();

        public async Task OnGetAsync()
        {
            var r = await _api.GetAsync<List<RolDto>>("api/roles");
            if (r.Exito && r.Datos != null) Roles = r.Datos;
            else MostrarError(r.Error);
        }
    }
}
