using NexusOps.Web.Models;

namespace NexusOps.Web.Helpers
{
    // Transiciones que la interfaz ofrece como botones. La API valida de nuevo (validación definitiva).
    public static class FlujoEstadosUi
    {
        public static IReadOnlyList<string> Incidencia(string estado, bool esTecnico) => estado switch
        {
            EstadosIncidencia.Registrada => esTecnico ? Array.Empty<string>() : new[] { EstadosIncidencia.Asignada, EstadosIncidencia.Cancelada },
            EstadosIncidencia.Asignada => esTecnico ? new[] { EstadosIncidencia.EnProceso } : new[] { EstadosIncidencia.EnProceso, EstadosIncidencia.Cancelada },
            EstadosIncidencia.EnProceso => esTecnico ? new[] { EstadosIncidencia.Resuelta } : new[] { EstadosIncidencia.Resuelta, EstadosIncidencia.Cancelada },
            EstadosIncidencia.Resuelta => esTecnico ? Array.Empty<string>() : new[] { EstadosIncidencia.Cerrada, EstadosIncidencia.EnProceso },
            _ => Array.Empty<string>()
        };

        public static IReadOnlyList<string> Tarea(string estadoRegistrado, bool esTecnico) => estadoRegistrado switch
        {
            EstadosTarea.Pendiente => esTecnico ? new[] { EstadosTarea.EnProceso, EstadosTarea.Completada } : new[] { EstadosTarea.EnProceso, EstadosTarea.Completada, EstadosTarea.Cancelada },
            EstadosTarea.EnProceso => esTecnico ? new[] { EstadosTarea.Completada } : new[] { EstadosTarea.Completada, EstadosTarea.Cancelada },
            _ => Array.Empty<string>()
        };

        public static IReadOnlyList<string> Mantenimiento(string estado, bool esTecnico) => estado switch
        {
            EstadosMantenimiento.Programado => esTecnico ? new[] { EstadosMantenimiento.EnProceso } : new[] { EstadosMantenimiento.EnProceso, EstadosMantenimiento.Cancelado },
            EstadosMantenimiento.EnProceso => esTecnico ? new[] { EstadosMantenimiento.Completado } : new[] { EstadosMantenimiento.Completado, EstadosMantenimiento.Cancelado },
            _ => Array.Empty<string>()
        };

        // Texto del botón que ejecuta la transición.
        public static string Accion(string estadoDestino) => estadoDestino switch
        {
            EstadosIncidencia.Asignada => "Asignar",
            EstadosIncidencia.EnProceso => "Iniciar",
            EstadosIncidencia.Resuelta => "Marcar resuelta",
            EstadosIncidencia.Cerrada => "Cerrar",
            EstadosIncidencia.Cancelada => "Cancelar",
            EstadosTarea.Completada => "Completar",
            EstadosMantenimiento.Completado => "Completar",
            EstadosMantenimiento.Cancelado => "Cancelar",
            _ => estadoDestino
        };

        public static string Icono(string estadoDestino) => estadoDestino switch
        {
            EstadosIncidencia.Asignada => "bi-person-check",
            EstadosIncidencia.EnProceso => "bi-play-circle",
            EstadosIncidencia.Resuelta or EstadosTarea.Completada or EstadosMantenimiento.Completado => "bi-check-circle",
            EstadosIncidencia.Cerrada => "bi-lock",
            EstadosIncidencia.Cancelada or EstadosMantenimiento.Cancelado => "bi-x-circle",
            _ => "bi-arrow-right-circle"
        };

        public static string ClaseBoton(string estadoDestino) => estadoDestino switch
        {
            EstadosIncidencia.Cancelada or EstadosMantenimiento.Cancelado => "btn-outline-danger",
            EstadosIncidencia.Resuelta or EstadosTarea.Completada or EstadosMantenimiento.Completado => "btn-success",
            _ => "btn-primary"
        };

        // Estados que exigen escribir un motivo.
        public static bool RequiereComentario(string estadoDestino) =>
            estadoDestino == EstadosIncidencia.Cancelada || estadoDestino == EstadosMantenimiento.Cancelado;
    }
}
