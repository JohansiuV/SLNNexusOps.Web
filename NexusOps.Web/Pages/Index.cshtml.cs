using Microsoft.AspNetCore.Mvc;
using NexusOps.Web.Helpers;

namespace NexusOps.Web.Pages
{
    // Página de entrada: envía a cada rol a su pantalla principal.
    public class IndexModel : NxPageModel
    {
        public IActionResult OnGet() =>
            EsTecnico ? RedirectToPage("/Incidencias/Index") : RedirectToPage("/Dashboard/Index");
    }
}
