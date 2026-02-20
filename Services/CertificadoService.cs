using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LMS.Services
{
    public class CertificadoService
    {
        private readonly IWebHostEnvironment _env;

        public CertificadoService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] GenerarCertificado(string nombreEstudiante, string nombreCurso)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var escudoPath = Path.Combine(_env.WebRootPath, "images", "escudo.jpg");
            var logoPath = Path.Combine(_env.WebRootPath, "images", "logo.jpeg");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);

                    page.Content()
                        .Border(3)
                        .BorderColor(Colors.Blue.Darken2)
                        .Padding(30)
                        .Column(col =>
                        {
                            col.Spacing(15);

                            // 🔹 Parte superior normal
                            col.Item().AlignCenter().Height(80).Image(escudoPath);

                            col.Item().AlignCenter().Text("República de Costa Rica")
                                .FontSize(22);

                            col.Item().AlignCenter().Height(70).Image(logoPath);

                            col.Item().AlignCenter().Text("Entrega a:")
                                .FontSize(16).Bold();

                            col.Item().AlignCenter().Text(nombreEstudiante)
                                .FontSize(36)
                                .Bold();

                            col.Item().AlignCenter().LineHorizontal(1);

                            col.Item().AlignCenter().Text("Por haber concluido satisfactoriamente el curso")
                                .FontSize(14);

                            col.Item().AlignCenter().PaddingTop(10).Text(nombreCurso)
                                .FontSize(24)
                                .Bold();

                            // 🔥 ESTA ES LA CLAVE
                            col.Item().ExtendVertical().AlignBottom().Row(row =>
                            {
                                row.RelativeItem(3).Column(c =>
                                {
                                    c.Item().LineHorizontal(1);
                                    c.Item().AlignCenter()
                                        .Text("Directores del Programa Integrado")
                                        .FontSize(12);
                                });

                                row.RelativeItem(1).Column(c =>
                                {
                                    c.Item().LineHorizontal(1);
                                    c.Item().AlignCenter()
                                        .Text(DateTime.Now.ToString("dd-MM-yyyy"))
                                        .FontSize(12);

                                    c.Item().AlignCenter()
                                        .Text("Fecha")
                                        .FontSize(10);
                                });
                            });
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}