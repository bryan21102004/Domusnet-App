using System.Text.RegularExpressions;

namespace DomusNet.API.Services.ReglasNegocio;

public class ValidacionesGenerales
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,10}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex TelefonoCostaRicaRegex = new(
        @"^[2-9]\d{7}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex MayusculaRegex = new(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex MinusculaRegex = new(@"[a-z]", RegexOptions.Compiled);
    private static readonly Regex NumeroRegex    = new(@"[0-9]", RegexOptions.Compiled);

    // ─── texto ───────────────────────────────────────────────────────────────

    public void ValidarTexto(string? valor, string nombreCampo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException($"{nombreCampo} es obligatorio.");
    }

    public void ValidarLongitudMinima(string? valor, string nombreCampo, int minimo)
    {
        ValidarTexto(valor, nombreCampo);
        if (valor!.Trim().Length < minimo)
            throw new ArgumentException($"{nombreCampo} debe tener al menos {minimo} caracteres.");
    }

    public void ValidarLongitudMaxima(string? valor, string nombreCampo, int maximo)
    {
        if (valor != null && valor.Trim().Length > maximo)
            throw new ArgumentException($"{nombreCampo} no puede superar los {maximo} caracteres.");
    }

    // ─── id ──────────────────────────────────────────────────────────────────

    public void ValidarId(int id, string nombreCampo = "ID")
    {
        if (id <= 0)
            throw new ArgumentException($"{nombreCampo} no es válido.");
    }

    // ─── correo ──────────────────────────────────────────────────────────────

    public void ValidarCorreo(string correo)
    {
        ValidarTexto(correo, "Correo");
        if (!EmailRegex.IsMatch(correo.Trim()))
            throw new ArgumentException("El correo electrónico no tiene un formato válido.");
    }

    public void ValidarCorreoOpcional(string? correo)
    {
        if (string.IsNullOrWhiteSpace(correo)) return;
        if (!EmailRegex.IsMatch(correo.Trim()))
            throw new ArgumentException("El correo electrónico no tiene un formato válido.");
    }

    // ─── teléfono ────────────────────────────────────────────────────────────

    public void ValidarTelefono(string? telefono)
    {
        ValidarTexto(telefono, "Teléfono");
        var soloDigitos = Regex.Replace(telefono!.Trim(), @"\D", string.Empty);
        if (soloDigitos.StartsWith("506") && soloDigitos.Length >= 11)
            soloDigitos = soloDigitos[^8..];
        if (soloDigitos.Length != 8 || !TelefonoCostaRicaRegex.IsMatch(soloDigitos))
            throw new ArgumentException("El teléfono debe tener 8 dígitos válidos para Costa Rica (inicia con 2-9).");
    }

    // ─── descripción ─────────────────────────────────────────────────────────

    public void ValidarDescripcion(string? descripcion, string nombreCampo = "Descripción",
        int minimo = 10, int maximo = 1000)
    {
        ValidarLongitudMinima(descripcion, nombreCampo, minimo);
        ValidarLongitudMaxima(descripcion, nombreCampo, maximo);
    }

    // ─── fecha ───────────────────────────────────────────────────────────────

    public void ValidarFechaFutura(DateTime fecha, string nombreCampo = "Fecha")
    {
        if (fecha <= DateTime.Now)
            throw new ArgumentException($"{nombreCampo} debe ser una fecha y hora posterior a la actual.");
    }

    // ─── contraseña ──────────────────────────────────────────────────────────

    public void ValidarContrasenaSegura(string? password)
    {
        ValidarTexto(password, "Contraseña");
        if (password!.Length < 8)
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.");
        if (!MayusculaRegex.IsMatch(password))
            throw new ArgumentException("La contraseña debe contener al menos una letra mayúscula.");
        if (!MinusculaRegex.IsMatch(password))
            throw new ArgumentException("La contraseña debe contener al menos una letra minúscula.");
        if (!NumeroRegex.IsMatch(password))
            throw new ArgumentException("La contraseña debe contener al menos un número.");
    }
}
