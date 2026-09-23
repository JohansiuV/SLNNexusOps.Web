using Microsoft.AspNetCore.Mvc.Rendering;

namespace NexusOps.Web.Models
{
    // Configuración del modal de cambio de estado (partial _ModalEstado).
    public record ModalEstadoInfo(string Handler = "Estado", IEnumerable<SelectListItem>? Usuarios = null);
}
