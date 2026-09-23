namespace NexusOps.Web.Models
{
    // Valores permitidos por la base de datos (restricciones CHECK) y utilizados por la API.

    public static class RolesSistema
    {
        public const string Administrador = "Administrador";
        public const string Supervisor = "Supervisor";
        public const string Tecnico = "Técnico";
        public const string AdminSupervisor = Administrador + "," + Supervisor;
    }

    public static class EstadosActivo
    {
        public const string Operativo = "Operativo";
        public const string EnMantenimiento = "En mantenimiento";
        public const string Danado = "Dañado";
        public const string Inactivo = "Inactivo";
        public const string DadoDeBaja = "Dado de baja";

        public static readonly string[] Todos = { Operativo, EnMantenimiento, Danado, Inactivo, DadoDeBaja };
    }

    public static class Prioridades
    {
        public const string Baja = "Baja";
        public const string Media = "Media";
        public const string Alta = "Alta";
        public const string Critica = "Critica";

        public static readonly string[] Todas = { Baja, Media, Alta, Critica };
    }

    public static class EstadosIncidencia
    {
        public const string Registrada = "Registrada";
        public const string Asignada = "Asignada";
        public const string EnProceso = "En proceso";
        public const string Resuelta = "Resuelta";
        public const string Cerrada = "Cerrada";
        public const string Cancelada = "Cancelada";

        public static readonly string[] Todos = { Registrada, Asignada, EnProceso, Resuelta, Cerrada, Cancelada };
        public static readonly string[] Abiertos = { Registrada, Asignada, EnProceso };
    }

    public static class EstadosTarea
    {
        public const string Pendiente = "Pendiente";
        public const string EnProceso = "En proceso";
        public const string Completada = "Completada";
        public const string Cancelada = "Cancelada";
        public const string Vencida = "Vencida"; // estado calculado

        public static readonly string[] Todos = { Pendiente, EnProceso, Completada, Cancelada, Vencida };
        public static readonly string[] SinCompletar = { Pendiente, EnProceso };
    }

    public static class TiposMantenimiento
    {
        public const string Preventivo = "Preventivo";
        public const string Correctivo = "Correctivo";

        public static readonly string[] Todos = { Preventivo, Correctivo };
    }

    public static class EstadosMantenimiento
    {
        public const string Programado = "Programado";
        public const string EnProceso = "En proceso";
        public const string Completado = "Completado";
        public const string Cancelado = "Cancelado";

        public static readonly string[] Todos = { Programado, EnProceso, Completado, Cancelado };
        public static readonly string[] Pendientes = { Programado, EnProceso };
    }

    public static class TiposAlerta
    {
        public const string IncidenciaCritica = "INC_CRIT";
        public const string IncidenciaPendiente = "INC_PEND";
        public const string MantenimientoProximo = "MANT_PROX";
        public const string MantenimientoVencido = "MANT_VENC";
        public const string TareaVencida = "TAR_VENC";
        public const string StockBajo = "STOCK_BAJO";
        public const string ActivoRecurrente = "ACT_RECURR";

        public static readonly string[] Todos =
        {
            IncidenciaCritica, IncidenciaPendiente, MantenimientoProximo, MantenimientoVencido,
            TareaVencida, StockBajo, ActivoRecurrente
        };
    }

    public static class NivelesAlerta
    {
        public const string Critica = "Critica";
        public const string Advertencia = "Advertencia";
        public const string Informativa = "Informativa";
    }

    public static class EntidadesHistorial
    {
        public const string Activo = "Activo";
        public const string Incidencia = "Incidencia";
        public const string Tarea = "Tarea";
        public const string Mantenimiento = "Mantenimiento";
    }
}
