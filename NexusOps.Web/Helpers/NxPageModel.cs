using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexusOps.Web.Models;

namespace NexusOps.Web.Helpers
{
    // Base de las páginas: mensajes de éxito/error y datos del usuario autenticado.
    public abstract class NxPageModel : PageModel
    {
        public int IdUsuarioActual =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        public string RolActual => User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        public bool EsAdministrador => RolActual == RolesSistema.Administrador;
        public bool EsSupervisor => RolActual == RolesSistema.Supervisor;
        public bool EsTecnico => RolActual == RolesSistema.Tecnico;
        public bool EsAdminOSupervisor => EsAdministrador || EsSupervisor;

        // Mensaje que sobrevive a una redirección (después de guardar).
        protected void Exito(string mensaje) => TempData["Exito"] = mensaje;

        protected void ErrorTemp(string mensaje) => TempData["Error"] = mensaje;

        // Mensaje para la respuesta actual (se muestra sin redirigir).
        protected void MostrarError(string? mensaje) =>
            ViewData["Error"] = mensaje ?? "Ocurrió un error al procesar la solicitud.";
    }
}
