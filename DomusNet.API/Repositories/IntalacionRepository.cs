using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.Models;
using DomusNet.API.Repositories.Interfaces;

namespace DomusNet.API.Repositories;

public class InstalacionRepository : IInstalacionRepository
{
    private readonly DomusNetDBContext _context;

    public InstalacionRepository(DomusNetDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TecnicoActivo>> ListarTecnicosActivosAsync()
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<TecnicoActivo>(
            "listarTecnicosActivos",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ResultadoOperacion?> ProgramarInstalacionAsync(ProgramarInstalacionRequest request)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<ResultadoOperacion>(
            "programarInstalacion",
            new
            {
                request.IdSolicitud,
                request.IdTecnicoAsignado,
                request.IdVendedorPrograma,
                request.FechaProgramada,
                request.UbicacionInstalacion,
                request.NotasVendedor
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<InstalacionTecnicoResponse>> ListarPorTecnicoAsync(int idTecnico)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<InstalacionTecnicoResponse>(
            "dbo.listarInstalacionesPorTecnico",
            new
            {
                IdTecnico = idTecnico
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ResultadoOperacion?> CompletarInstalacionAsync(CompletarInstalacionRequest request)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<ResultadoOperacion>(
            "completarInstalacion",
            new
            {
                request.IdInstalacion,
                request.FotoEvidencia,
                request.PruebaVelocidad,
                request.ComentarioTecnico
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<InstalacionGeneralResponse>> ListarTodasAsync()
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<InstalacionGeneralResponse>(
            "dbo.listarInstalacionesGenerales",
            commandType: CommandType.StoredProcedure);
    }
}