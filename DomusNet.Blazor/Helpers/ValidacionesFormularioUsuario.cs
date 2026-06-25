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

        var soloDigitos = Regex.Replace(telefono.Trim(), @"\D", string.Empty);

        if (soloDigitos.StartsWith("506") && soloDigitos.Length >= 11)
            soloDigitos = soloDigitos[^8..];

        if (soloDigitos.Length != 8)
            return (false, "El teléfono debe tener 8 dígitos (Costa Rica). Ejemplo: 7537-2729 o +50675372729.");

        if (!TelefonoOchoDigitosRegex.IsMatch(soloDigitos))
            return (false, "El teléfono no es válido para Costa Rica. Debe iniciar con 2, 4, 5, 6, 7 u 8.");

        return (true, string.Empty);
    }

    public static (bool Valido, string Mensaje) ValidarContrasena(string? password, int minLength = 6)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "La contraseña es obligatoria.");

        if (password.Length < minLength)
            return (false, $"La contraseña debe tener al menos {minLength} caracteres.");

        return (true, string.Empty);
    }

    public static string NormalizarTelefono(string? telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
            return string.Empty;

        var soloDigitos = Regex.Replace(telefono.Trim(), @"\D", string.Empty);

        if (soloDigitos.StartsWith("506") && soloDigitos.Length >= 11)
            soloDigitos = soloDigitos[^8..];

        return soloDigitos.Length == 8
            ? $"{soloDigitos[..4]}-{soloDigitos[4..]}"
            : telefono.Trim();
    }
}
