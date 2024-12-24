using PdfSharp.Drawing;
using System.Text;

public class PdfSignatureService
{
    public async Task ApplySignatureToPdf(string inputPath, string outputPath, byte[] signatureImage, float x, float y)
    {
        // Enable Unicode support
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        try
        {
            // Create a copy of original PDF first
            using (var sourceStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var destStream = new FileStream(outputPath, FileMode.Create))
            {
                await sourceStream.CopyToAsync(destStream);
            }

            // Create a new memory stream from signature bytes
            using (var signatureStream = new MemoryStream())
            {
                // Write signature bytes to memory stream
                await signatureStream.WriteAsync(signatureImage, 0, signatureImage.Length);
                signatureStream.Position = 0;

                // Modify PDF with signature
                using (var document = PdfSharp.Pdf.IO.PdfReader.Open(outputPath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Modify))
                {
                    var page = document.Pages[0];
                    using (var gfx = XGraphics.FromPdfPage(page))
                    {
                        var image = XImage.FromStream(signatureStream);
                        var yPos = page.Height.Point - y - 50;
                        gfx.DrawImage(image, x, yPos, 100, 50);
                    }
                    document.Save(outputPath);
                }
            }
        }
        catch (Exception ex)
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            throw new Exception($"Error applying signature to PDF: {ex.Message}", ex);
        }
    }
}
