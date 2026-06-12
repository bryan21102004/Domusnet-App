using System.Data;
using Dapper;
using DomusNet.API.Data;
using DomusNet.API.Models;

namespace DomusNet.API.Services;

public class InstalacionService
{
    private readonly DomusNetDBContext _context;

    public InstalacionService(DomusNetDBContext context)
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

    public async Task<ResultadoOperacion> ProgramarInstalacionAsync(ProgramarInstalacionRequest request)
    {
        using var connection = _context.CreateConnection();
        var resultado = await connection.QueryFirstOrDefaultAsync<ResultadoOperacion>(
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

        return resultado ?? new ResultadoOperacion { Resultado = -1, Mensaje = "No se obtuvo respuesta de la base de datos." };
    }

    public async Task<IEnumerable<InstalacionProgramada>> ListarPorTecnicoAsync(int idTecnico)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<InstalacionProgramada>(
            "listarInstalacionesPorTecnico",
            new { IdTecnico = idTecnico },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ResultadoOperacion> CompletarInstalacionAsync(CompletarInstalacionRequest request)
    {
        using var connection = _context.CreateConnection();
        var resultado = await connection.QueryFirstOrDefaultAsync<ResultadoOperacion>(
            "completarInstalacion",
            new
            {
                request.IdInstalacion,
                request.FotoEvidencia,
                request.PruebaVelocidad,
                request.ComentarioTecnico
            },
            commandType: CommandType.StoredProcedure);

        return resultado ?? new ResultadoOperacion { Resultado = -1, Mensaje = "No se obtuvo respuesta de la base de datos." };
    }
}