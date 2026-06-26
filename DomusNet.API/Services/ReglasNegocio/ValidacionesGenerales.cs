namespace DomusNet.API.Services.ReglasNegocio;

public class ValidacionesGenerales
{
    public void ValidarTexto(string? valor, string nombreCampo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new Exception($"{nombreCampo} es obligatorio.");
    }

    public void ValidarId(int id, string nombreCampo = "ID")
    {
        if (id <= 0)
            throw new Exception($"{nombreCampo} es invalido.");
    }

    public void ValidarCorreo(string correo)
    {
        ValidarTexto(correo, "Correo");
        if (!correo.Contains('@'))
            throw new Exception("Correo invalido.");
    }
}
