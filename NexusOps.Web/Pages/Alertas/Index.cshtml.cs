using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Helpers;
using NexusOps.Web.Models;
using NexusOps.Web.Services;

namespace NexusOps.Web.Pages.Alertas
{
    public class IndexModel : NxPageModel
    {
        private readonly ApiService _api;

        public IndexModel(ApiService api)
        {
            _api = api;
        }

        public PaginadoDto<AlertaDto> Resultado { get; set; } = new();
        public List<SelectListItem> Niveles { get; set; } = new();

        public bool SoloNoLeidas { get; set; }
        public string? Nivel { get; set; }

        public async Task OnGetAsync(bool soloNoLeidas = false, string? nivel = null, int pagina = 1)
        {
            SoloNoLeidas = soloNoLeidas;
            Nivel = nivel;
            Niveles = CatalogosService.Lista(new[] { NivelesAlerta.Critica, NivelesAlerta.Advertencia, NivelesAlerta.Informativa });

            var r = await _api.GetAsync<PaginadoDto<AlertaDto>>(ApiService.Url("api/alertas",
                ("soloNoLeidas", soloNoLeidas ? true : null), ("nivel", nivel), ("pagina", pagina), ("tamano", 15)));

            if (r.Exito && r.Datos != null) Resultado = r.Datos;
            else MostrarError(r.Error);
        }

        // GET /Alertas?handler=Contador  (lo consulta alertas.js para la campana)
        public async Task<IActionResult> OnGetContadorAsync()
        {
            var r = await _api.GetAsync<AlertaContadorDto>("api/alertas/contador");
            var datos = r.Datos ?? new AlertaContadorDto();
            return new JsonResult(new { noLeidas = datos.NoLeidas, criticas = datos.Criticas });
        }

        public async Task<IActionResult> OnPostLeerAsync(int id)
        {
            var r = await _api.PutAsync<object>($"api/alertas/{id}/leer");
            if (!r.Exito) ErrorTemp(r.Error ?? "No se pudo marcar la alerta como leída.");
            return RedirectToPage(new { soloNoLeidas = SoloNoLeidasDeQuery(), nivel = Request.Query["nivel"].ToString() });
        }

        public async Task<IActionResult> OnPostLeerTodasAsync()
        {
            var r = await _api.PostAsync<object>("api/alertas/leer-todas");
            if (r.Exito) Exito("✓ Todas las alertas fueron marcadas como leídas.");
            else ErrorTemp(r.Error ?? "No se pudieron marcar las alertas.");
            return RedirectToPage();
        }

        private bool SoloNoLeidasDeQuery() => Request.Query["soloNoLeidas"].ToString().Equals("true", StringComparison.OrdinalIgnoreCase);
    }
}
