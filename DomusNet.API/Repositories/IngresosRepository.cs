using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.Models;
using DomusNet.API.Repositories.Interfaces;

namespace DomusNet.API.Repositories;

public class IngresosRepository : IIngresosRepository
{
    private readonly DomusNetDBContext _context;

    public IngresosRepository(DomusNetDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClienteIngresoResponse>> ListarClientesParaIngresoAsync()
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<ClienteIngresoResponse>(
            "dbo.listarClientesParaIngreso",
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<ResultadoIngresoResponse?> RegistrarIngresoClienteAsync(RegistrarIngresoRequest request)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<ResultadoIngresoResponse>(
            "dbo.registrarIngresoCliente",
            new
            {
                request.IdCliente,
                request.Monto,
                request.MetodoPago,
                request.ReferenciaPago,
                request.Descripcion,
                request.IdRegistradoPor
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<IEnumerable<IngresoResponse>> ListarIngresosAsync()
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<IngresoResponse>(
            "dbo.listarIngresos",
            commandType: CommandType.StoredProcedure
        );
    }
}