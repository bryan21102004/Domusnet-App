namespace DomusNet.API.Data.Models;

public enum EstadoPaquete
{
    Activo,
    Inactivo
}

public enum EstadoSolicitud
{
    Pendiente,
    Atendida,
    Cancelada
}

public enum EstadoPago
{
    AlDia,
    Moroso,
    Pendiente
}

public enum EstadoAsignacion
{
    Activa,
    Suspendida,
    Cancelada
}

public enum TipoTicket
{
    Averia,
    Aviso,
    Recordatorio
}

public enum EstadoTicket
{
    Pendiente,
    EnProceso,
    Resuelto
}

public enum PrioridadTicket
{
    Alta,
    Media,
    Baja
}

public enum TipoReporte
{
    Ingresos,
    Clientes,
    Tickets,
    Operativo
}
