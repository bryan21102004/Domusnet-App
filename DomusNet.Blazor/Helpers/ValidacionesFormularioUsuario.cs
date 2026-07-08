using System.Net.Mail;
using System.Text.RegularExpressions;

namespace DomusNet.Blazor.Helpers;

public static class ValidacionesFormularioUsuario
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,10}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex TelefonoOchoDigitosRegex = new(
        @"^[2-9]\d{7}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly HashSet<string> ExtensionesCorreoSospechosas = new(StringComparer.OrdinalIgnoreCase)
    {
        "ocm", "con", "comm", "cmo", "orgn", "ent", "etl", "coom", "comn"
    };

    public static (bool Valido, string Mensaje) ValidarNombre(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return (false, "El nombre es obligatorio.");

        var valor = nombre.Trim();

        if (valor.Length < 3)
            return (false, "El nombre debe tener al menos 3 caracteres.");

        if (!Regex.IsMatch(valor, @"^[a-zA-ZÀ-ÿ\s'.-]+$"))
            return (false, "El nombre solo puede contener letras y espacios.");

        return (true, string.Empty);
    }

    public static (bool Valido, string Mensaje) ValidarCorreo(string? correo)
    {
        if (string.IsNullOrWhiteSpace(correo))
            return (false, "El correo es obligatorio.");

        var valor = correo.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(valor))
            return (false, "Ingresá un correo válido (ejemplo: usuario@domusnet.com).");

        if (!MailAddress.TryCreate(valor, out _))
            return (false, "El formato del correo no es válido.");

        var extension = valor.Split('@').LastOrDefault()?.Split('.').LastOrDefault();

        if (!string.IsNullOrWhiteSpace(extension) && ExtensionesCorreoSospechosas.Contains(extension))
            return (false, $"La extensión '.{extension}' parece incorrecta. Verificá que el correo esté bien escrito.");

        return (true, string.Empty);
    }

  public static (bool Valido, string Mensaje) ValidarTelefono(string? telefono)
{
    if (string.IsNullOrWhiteSpace(telefono))
        return (true, string.Empty);

    var valor = telefono.Trim();

    if (!Regex.IsMatch(valor, @"^\d{8}$"))
        return (false, "El teléfono debe contener solo números y tener exactamente 8 dígitos.");

    if (!TelefonoOchoDigitosRegex.IsMatch(valor))
        return (false, "El teléfono no es válido para Costa Rica. Debe iniciar con 2, 4, 5, 6, 7 u 8.");

    return (true, string.Empty);
}

    private static readonly Regex MayusculaRegex = new(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex MinusculaRegex = new(@"[a-z]", RegexOptions.Compiled);
    private static readonly Regex NumeroRegex    = new(@"[0-9]", RegexOptions.Compiled);

    public static (bool Valido, string Mensaje) ValidarContrasena(string? password, int minLength = 6)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "La contraseña es obligatoria.");

        if (password.Length < minLength)
            return (false, $"La contraseña debe tener al menos {minLength} caracteres.");

        return (true, string.Empty);
    }

    public static (bool Valido, string Mensaje) ValidarContrasenaSegura(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "La contraseña es obligatoria.");
        if (password.Length < 8)
            return (false, "La contraseña debe tener al menos 8 caracteres.");
        if (!MayusculaRegex.IsMatch(password))
            return (false, "La contraseña debe contener al menos una letra mayúscula.");
        if (!MinusculaRegex.IsMatch(password))
            return (false, "La contraseña debe contener al menos una letra minúscula.");
        if (!NumeroRegex.IsMatch(password))
            return (false, "La contraseña debe contener al menos un número.");
        return (true, string.Empty);
    }

    public static (bool Valido, string Mensaje) ValidarTelefonoObligatorio(string? telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
            return (false, "El teléfono es obligatorio.");
        return ValidarTelefono(telefono);
    }

    public static (bool Valido, string Mensaje) ValidarLongitudMinima(string? valor, string nombreCampo, int minimo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return (false, $"{nombreCampo} es obligatorio.");
        if (valor.Trim().Length < minimo)
            return (false, $"{nombreCampo} debe tener al menos {minimo} caracteres.");
        return (true, string.Empty);
    }

    public static (bool Valido, string Mensaje) ValidarLongitudMaxima(string? valor, string nombreCampo, int maximo)
    {
        if (valor != null && valor.Trim().Length > maximo)
            return (false, $"{nombreCampo} no puede superar los {maximo} caracteres.");
        return (true, string.Empty);
    }

    public static (bool Valido, string Mensaje) ValidarDireccion(string? direccion)
        => ValidarLongitudMinima(direccion, "La dirección", 10);

public static string NormalizarTelefono(string? telefono)
{
    if (string.IsNullOrWhiteSpace(telefono))
        return string.Empty;

    return telefono.Trim();
}
}
