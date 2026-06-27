using DomusNet.API.Models;
using DomusNet.API.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DomusNet.API.Services;

public class ReportePdfService : IReportePdfService
{
    private readonly IReportesService _reportesService;

    public ReportePdfService(IReportesService reportesService)
    {
        _reportesService = reportesService;
    }

    public async Task<ReportePdfResultado?> GenerarReporteIngresosPdfAsync(int idIngresoMensual)
    {
        var detalle = await _reportesService.ObtenerDetalleReporteIngresoAsync(idIngresoMensual);

        if (detalle?.Resumen == null)
        {
            return null;
        }

        var resumen = detalle.Resumen;

        var pdf = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(header =>
                {
                    header.Item().Text("DomusNet Costa Rica")
                        .FontSize(22)
                        .Bold()
                        .FontColor(Colors.Blue.Medium);

                    header.Item().Text("Reporte financiero de ingresos")
                        .FontSize(14)
                        .SemiBold();

                    header.Item().Text($"Reporte #{resumen.IdIngresoMensual} - {NombreMes(resumen.Mes)} {resumen.Anio}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);

                    header.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                });

                page.Content().PaddingTop(15).Column(content =>
                {
                    content.Spacing(12);

                    content.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => Card(c, "Total bruto", Dinero(resumen.MontoTotalBruto)));
                        row.RelativeItem().Element(c => Card(c, "Con rebajas", Dinero(resumen.MontoTotalConRebajas)));
                        row.RelativeItem().Element(c => Card(c, "DomusNet", Dinero(resumen.MontoDomusNet)));
                        row.RelativeItem().Element(c => Card(c, "Trabajadores", Dinero(resumen.MontoTrabajadores)));
                    });

                    content.Item().Text("Resumen general").FontSize(14).Bold();

                    content.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        InfoRow(table, "Mes", NombreMes(resumen.Mes));
                        InfoRow(table, "Año", resumen.Anio.ToString());
                        InfoRow(table, "Quincena", string.IsNullOrWhiteSpace(resumen.Quincena) ? "Mes completo" : resumen.Quincena);
                        InfoRow(table, "Registrado por", resumen.RegistradoPor ?? "-");
                        InfoRow(table, "Fecha registro", resumen.FechaRegistro.ToString("dd/MM/yyyy HH:mm"));
                        InfoRow(table, "IVA", $"{resumen.PorcentajeIVA:N2}%");
                        InfoRow(table, "Cruz Roja", $"{resumen.PorcentajeCruzRoja:N2}%");
                        InfoRow(table, "911", $"{resumen.Porcentaje911:N2}%");
                        InfoRow(table, "DomusNet", $"{resumen.PorcentajeDomusNet:N2}%");
                        InfoRow(table, "Trabajadores", $"{resumen.PorcentajeTrabajadores:N2}%");
                    });

                    if (detalle.TotalesPorTipo.Any())
                    {
                        content.Item().Text("Totales por tipo").FontSize(14).Bold();

                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            HeaderCell(table.Cell(), "Tipo");
                            HeaderCell(table.Cell(), "Total asignado");

                            foreach (var item in detalle.TotalesPorTipo)
                            {
                                BodyCell(table.Cell(), item.TipoDistribucion ?? "-");
                                BodyCell(table.Cell(), Dinero(item.TotalAsignado));
                            }
                        });
                    }

                    content.Item().Text("Distribución por trabajador").FontSize(14).Bold();

                    content.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        HeaderCell(table.Cell(), "Trabajador");
                        HeaderCell(table.Cell(), "Tipo");
                        HeaderCell(table.Cell(), "Porcentaje");
                        HeaderCell(table.Cell(), "Monto");

                        foreach (var item in detalle.Distribuciones)
                        {
                            BodyCell(table.Cell(), item.Trabajador ?? "-");
                            BodyCell(table.Cell(), item.TipoDistribucion ?? "-");
                            BodyCell(table.Cell(), $"{item.PorcentajeAplicado:N2}%");
                            BodyCell(table.Cell(), Dinero(item.MontoAsignado));
                        }
                    });

                    if (!string.IsNullOrWhiteSpace(resumen.Notas))
                    {
                        content.Item().Text("Notas").FontSize(14).Bold();
                        content.Item().Text(resumen.Notas);
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();

        return new ReportePdfResultado
        {
            Archivo = pdf,
            NombreArchivo = $"reporte-ingresos-{resumen.IdIngresoMensual}.pdf"
        };
    }

    private static void Card(IContainer container, string titulo, string valor)
    {
        container
            .PaddingRight(6)
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(8)
            .Column(column =>
            {
                column.Item().Text(titulo).FontSize(9).FontColor(Colors.Grey.Darken1);
                column.Item().Text(valor).FontSize(12).Bold();
            });
    }

    private static void InfoRow(TableDescriptor table, string etiqueta, string valor)
    {
        BodyCell(table.Cell(), etiqueta, true);
        BodyCell(table.Cell(), valor);
    }

    private static void HeaderCell(IContainer container, string texto)
    {
        container
            .Background(Colors.Grey.Lighten3)
            .Border(1)
            .BorderColor(Colors.Grey.Lighten1)
            .Padding(5)
            .Text(texto)
            .FontSize(9)
            .Bold();
    }

    private static void BodyCell(IContainer container, string texto, bool destacado = false)
    {
        if (destacado)
        {
            container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(5)
                .Text(texto)
                .FontSize(9)
                .Bold();

            return;
        }

        container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5)
            .Text(texto)
            .FontSize(9);
    }

    private static string Dinero(decimal monto)
    {
        return $"₡{monto:N2}";
    }

    private static string NombreMes(int mes)
    {
        return mes switch
        {
            1 => "Enero",
            2 => "Febrero",
            3 => "Marzo",
            4 => "Abril",
            5 => "Mayo",
            6 => "Junio",
            7 => "Julio",
            8 => "Agosto",
            9 => "Septiembre",
            10 => "Octubre",
            11 => "Noviembre",
            12 => "Diciembre",
            _ => mes.ToString()
        };
    }
}