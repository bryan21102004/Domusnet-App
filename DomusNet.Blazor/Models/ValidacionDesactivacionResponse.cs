namespace DomusNet.Blazor.Models;

public class ValidacionDesactivacionResponse
{
    public bool TieneActividadPendiente { get; set; }
    public List<string> Detalles { get; set; } = [];
}
