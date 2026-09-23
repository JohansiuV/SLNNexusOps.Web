using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using NexusOps.Web.Models;

namespace NexusOps.Web.Helpers
{
    public record Miga(string Texto, string? Url = null);

    public record PaginacionInfo(int Pagina, int TotalPaginas, int Total, int Tamano);

    public static class UiHelper
    {
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        // ---------- Formato ----------
        public static string Moneda(decimal valor) => "S/ " + valor.ToString("N2", Inv);

        public static string Fecha(DateTime? fecha) => fecha.HasValue ? fecha.Value.ToString("dd/MM/yyyy", Inv) : "-";

        public static string FechaHora(DateTime? fecha) => fecha.HasValue ? fecha.Value.ToString("dd/MM/yyyy HH:mm", Inv) : "-";

        public static string FechaInput(DateTime? fecha) => fecha.HasValue ? fecha.Value.ToString("yyyy-MM-dd", Inv) : string.Empty;

        // "Critica" se guarda sin tilde en la base de datos; en pantalla se muestra "Crítica".
        public static string Texto(string? valor) => valor == Prioridades.Critica ? "Crítica" : (valor ?? string.Empty);

        public static string Iniciales(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return "?";
            var partes = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return partes.Length == 1 ? partes[0][..1].ToUpperInvariant()
                                      : (partes[0][..1] + partes[^1][..1]).ToUpperInvariant();
        }

        // ---------- Insignias (badges) ----------
        public static string ClasePrioridad(string? prioridad) => prioridad switch
        {
            Prioridades.Baja => "text-bg-secondary",
            Prioridades.Media => "text-bg-info",
            Prioridades.Alta => "text-bg-warning",
            Prioridades.Critica => "text-bg-danger",
            _ => "text-bg-light"
        };

        public static string ClaseEstado(string? estado) => estado switch
        {
            // Activos
            EstadosActivo.Operativo => "text-bg-success",
            EstadosActivo.EnMantenimiento => "text-bg-warning",
            EstadosActivo.Danado => "text-bg-danger",
            EstadosActivo.Inactivo => "text-bg-secondary",
            EstadosActivo.DadoDeBaja => "text-bg-dark",
            // Incidencias
            EstadosIncidencia.Registrada => "text-bg-secondary",
            EstadosIncidencia.Asignada => "text-bg-info",
            EstadosIncidencia.EnProceso => "text-bg-primary",
            EstadosIncidencia.Resuelta => "text-bg-success",
            EstadosIncidencia.Cerrada => "text-bg-dark",
            EstadosIncidencia.Cancelada => "text-bg-secondary",
            // Tareas
            EstadosTarea.Pendiente => "text-bg-secondary",
            EstadosTarea.Completada => "text-bg-success",
            EstadosTarea.Vencida => "text-bg-danger",
            // Mantenimientos
            EstadosMantenimiento.Programado => "text-bg-info",
            EstadosMantenimiento.Completado => "text-bg-success",
            EstadosMantenimiento.Cancelado => "text-bg-secondary",
            _ => "text-bg-light"
        };

        public static string ClaseNivelAlerta(string? nivel) => nivel switch
        {
            NivelesAlerta.Critica => "danger",
            NivelesAlerta.Advertencia => "warning",
            _ => "info"
        };

        public static string IconoNivel(string? nivel) => nivel switch
        {
            NivelesAlerta.Critica => "bi-exclamation-octagon-fill",
            NivelesAlerta.Advertencia => "bi-exclamation-triangle-fill",
            _ => "bi-info-circle-fill"
        };

        // Emoji equivalente (el enunciado del proyecto usa 🔴 / ⚠️ / 🟡).
        public static string EmojiNivel(string? nivel) => nivel switch
        {
            NivelesAlerta.Critica => "🔴",
            NivelesAlerta.Advertencia => "⚠️",
            _ => "ℹ️"
        };

        // Página de detalle relacionada con una alerta.
        public static string? EnlaceAlerta(AlertaDto a) => a.Entidad switch
        {
            EntidadesHistorial.Incidencia when a.IdEntidad.HasValue => $"/Incidencias/Detalle/{a.IdEntidad}",
            EntidadesHistorial.Mantenimiento when a.IdEntidad.HasValue => $"/Mantenimientos/Detalle/{a.IdEntidad}",
            EntidadesHistorial.Activo when a.IdEntidad.HasValue => $"/Activos/Detalle/{a.IdEntidad}",
            EntidadesHistorial.Tarea => "/Tareas?estado=Vencida",
            "Repuesto" => "/Repuestos?soloStockBajo=true",
            _ => null
        };

        // ---------- Paginación ----------
        // Reconstruye la URL actual cambiando solo el número de página.
        public static string UrlPagina(HttpRequest peticion, int pagina)
        {
            var sb = new StringBuilder(peticion.Path.Value);
            var primero = true;

            foreach (var par in peticion.Query)
            {
                if (par.Key.Equals("pagina", StringComparison.OrdinalIgnoreCase)) continue;
                foreach (var valor in par.Value)
                {
                    sb.Append(primero ? '?' : '&');
                    primero = false;
                    sb.Append(Uri.EscapeDataString(par.Key)).Append('=').Append(Uri.EscapeDataString(valor ?? string.Empty));
                }
            }

            sb.Append(primero ? '?' : '&').Append("pagina=").Append(pagina);
            return sb.ToString();
        }

        // Marca como seleccionado el valor indicado en una lista.
        public static List<SelectListItem> Seleccionar(IEnumerable<SelectListItem> items, string? valor) =>
            items.Select(i => new SelectListItem(i.Text, i.Value, i.Value == valor)).ToList();
    }
}
