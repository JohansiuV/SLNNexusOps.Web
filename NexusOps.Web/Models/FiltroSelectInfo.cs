using Microsoft.AspNetCore.Mvc.Rendering;

namespace NexusOps.Web.Models
{
    // Lista desplegable de un formulario de filtros (partial _FiltroSelect).
    public record FiltroSelectInfo(string Nombre, string Etiqueta, string? Valor, IEnumerable<SelectListItem> Items);
}
