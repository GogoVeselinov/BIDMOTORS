using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Project.Data;
using Project.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Project.Services
{
    public class InvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(Guid repairId)
        {
            var repair = await _context.Repairs
                .Include(r => r.Client)
                .Include(r => r.Car)
                .Include(r => r.UsedParts)
                    .ThenInclude(up => up.Part)
                .FirstOrDefaultAsync(r => r.Id == repairId);

            if (repair == null || repair.Status != "Completed" || repair.InvoiceNumber == null)
            {
                throw new InvalidOperationException("Ремонтът не е завършен или няма генериран номер на фактура");
            }

            var settings = await _context.PriceSettings.FirstOrDefaultAsync(s => s.IsActive);

            if (settings == null)
            {
                throw new InvalidOperationException("Няма активни настройки за цени");
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Element(c => ComposeHeader(c, repair, settings));
                    page.Content().Element(c => ComposeContent(c, repair, settings));
                    page.Footer().Element(c => ComposeFooter(c));
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container, Repair repair, PriceSettings settings)
        {
            container.Column(column =>
            {
                // Company info
                column.Item().Background("#f6d201").Padding(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(settings.CompanyName).FontSize(18).Bold().FontColor("#000");
                        col.Item().Text($"ЕИК: {settings.CompanyRegistrationNumber}").FontSize(9).FontColor("#000");
                        col.Item().Text($"ДДС №: {settings.CompanyVATNumber}").FontSize(9).FontColor("#000");
                    });
                });

                column.Item().PaddingVertical(5);

                // Invoice title and number
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("ФАКТУРА").FontSize(16).Bold();
                        col.Item().Text($"№ {repair.InvoiceNumber}").FontSize(12);
                        col.Item().Text($"Дата: {repair.InvoiceGeneratedOn:dd.MM.yyyy HH:mm}").FontSize(10);
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text("Данни за клиент:").FontSize(10).Bold();
                        col.Item().Text(repair.Client.Name).FontSize(10);
                        col.Item().Text($"Email: {repair.Client.Email}").FontSize(9);
                        col.Item().Text($"Телефон: {repair.Client.Phone}").FontSize(9);
                    });
                });

                column.Item().PaddingVertical(5);

                // Car info
                column.Item().Background("#f5f5f5").Padding(10).Column(col =>
                {
                    col.Item().Text("Автомобил").FontSize(11).Bold();
                    col.Item().Text($"{repair.Car.Brand} {repair.Car.Model} ({repair.Car.Year})").FontSize(10);
                    col.Item().Text($"Рег. №: {repair.Car.RegistrationNumber}").FontSize(9);
                });

                column.Item().PaddingVertical(10);
            });
        }

        private void ComposeContent(IContainer container, Repair repair, PriceSettings settings)
        {
            container.Column(column =>
            {
                // Manager notes (priority)
                if (!string.IsNullOrEmpty(repair.ManagerNotes))
                {
                    column.Item().Text("Описание на работата:").FontSize(11).Bold();
                    column.Item().Text(repair.ManagerNotes).FontSize(10);
                    column.Item().PaddingVertical(5);
                }
                // Fallback to work description if no manager notes
                else if (!string.IsNullOrEmpty(repair.WorkDescription))
                {
                    column.Item().Text("Описание на работата:").FontSize(11).Bold();
                    column.Item().Text(repair.WorkDescription).FontSize(10);
                    column.Item().PaddingVertical(5);
                }

                // Labor
                column.Item().Text("Услуги").FontSize(12).Bold();
                column.Item().PaddingVertical(3);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Background("#f6d201").Padding(5).Text("Описание").Bold();
                        header.Cell().Background("#f6d201").Padding(5).Text("Количество").Bold();
                        header.Cell().Background("#f6d201").Padding(5).Text("Цена").Bold();
                        header.Cell().Background("#f6d201").Padding(5).Text("Общо").Bold();
                    });

                    // Labor row
                    table.Cell().BorderBottom(0.5f).BorderColor("#ddd").Padding(5)
                        .Text($"Труд ({repair.LaborHours:F2} часа)");
                    table.Cell().BorderBottom(0.5f).BorderColor("#ddd").Padding(5)
                        .Text($"{repair.LaborHours:F2}");
                    table.Cell().BorderBottom(0.5f).BorderColor("#ddd").Padding(5)
                        .Text($"{settings.LaborCostPerHour:F2} лв");
                    table.Cell().BorderBottom(0.5f).BorderColor("#ddd").Padding(5)
                        .Text($"{repair.LaborCost:F2} лв").Bold();
                });

                column.Item().PaddingVertical(5);

                // Parts table
                if (repair.UsedParts != null && repair.UsedParts.Any())
                {
                    column.Item().Text("Части").FontSize(12).Bold();
                    column.Item().PaddingVertical(3);

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background("#f6d201").Padding(5).Text("Част").Bold();
                            header.Cell().Background("#f6d201").Padding(5).Text("Брой").Bold();
                            header.Cell().Background("#f6d201").Padding(5).Text("Ед. цена").Bold();
                            header.Cell().Background("#f6d201").Padding(5).Text("Общо").Bold();
                        });

                        // Parts rows
                        foreach (var usedPart in repair.UsedParts)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor("#ddd").Padding(5)
                                .Text(usedPart.Part.Name);
                            table.Cell().BorderBottom(0.5f).BorderColor("#ddd").Padding(5)
                                .Text($"{usedPart.QuantityUsed}");
                            table.Cell().BorderBottom(0.5f).BorderColor("#ddd").Padding(5)
                                .Text($"{usedPart.UnitPriceAtMoment:F2} лв");
                            table.Cell().BorderBottom(0.5f).BorderColor("#ddd").Padding(5)
                                .Text($"{usedPart.TotalPrice:F2} лв");
                        }
                    });

                    column.Item().PaddingVertical(5);
                }

                // Totals
                column.Item().AlignRight().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Междинна сума:").FontSize(11);
                        row.ConstantItem(100).AlignRight().Text($"{repair.TotalCost:F2} лв").FontSize(11);
                    });

                    col.Item().PaddingVertical(2);

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"ДДС ({settings.VATPercent}%):").FontSize(11);
                        var vat = repair.TotalCost * settings.VATPercent / 100;
                        row.ConstantItem(100).AlignRight().Text($"{vat:F2} лв").FontSize(11);
                    });

                    col.Item().PaddingVertical(5);

                    col.Item().Background("#f6d201").Padding(8).Row(row =>
                    {
                        row.RelativeItem().Text("ОБЩО ЗА ПЛАЩАНЕ:").FontSize(12).Bold();
                        var totalWithVat = repair.TotalCost * (1 + settings.VATPercent / 100);
                        row.ConstantItem(100).AlignRight().Text($"{totalWithVat:F2} лв").FontSize(14).Bold();
                    });
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Column(column =>
            {
                column.Item().PaddingTop(20).Text("Благодарим Ви за доверието!").FontSize(10).Italic();
                column.Item().Text(text =>
                {
                    text.Span("Документът е генериран автоматично на ").FontSize(8).FontColor("#999");
                    text.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm")).FontSize(8).FontColor("#999");
                });
            });
        }
    }
}
