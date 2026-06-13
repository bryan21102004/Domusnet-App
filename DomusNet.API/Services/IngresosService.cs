using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.Models;

namespace DomusNet.API.Services;

public class IngresosService
{
    private readonly DomusNetDBContext _context;

    public IngresosService(DomusNetDBContext context)
    {
        _context = context;
    }

    public async Task<ResultadoOperacion> RegistrarIngresoAsync(RegistrarIngresoRequest request)
    {
        using var connection = _context.CreateConnection();

        var resultado = await connection.QueryFirstOrDefaultAsync<ResultadoOperacion>(
            "registrarIngresoManual",
            new
            {
                request.IdCliente,
                request.IdPaquete,
                request.Monto,
                request.Fecha,
                request.Descripcion,
                request.IdRegistradoPor,
                request.TipoIngreso,
                request.MetodoPago,
                request.Quincena
            },
            commandType: CommandType.StoredProcedure);

        return resultado ?? new ResultadoOperacion
        {
            Resultado = -1,
            Mensaje = "No se obtuvo respuesta de la base de datos."
        };
    }

    public async Task<IEnumerable<IngresoListado>> ListarIngresosAsync(
        int? mes,
        int? anio,
        string? quincena,
        string? estado)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<IngresoListado>(
            "listarIngresos",
            new
            {
                Mes = mes,
                Anio = anio,
                Quincena = quincena,
                Estado = estado
            },
            commandType: CommandType.StoredProcedure);
    }

    
}