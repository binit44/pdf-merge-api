using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace PdfMergeApp.Services
{
    public class PdfMergeService
    {
        public void Merge(string pdf1Path, string pdf2Path, string outputPath)
        {
            using PdfWriter writer = new PdfWriter(outputPath);
            using PdfDocument mergedPdf = new PdfDocument(writer);

            PdfMerger merger = new PdfMerger(mergedPdf);

            using PdfDocument pdf1 = new PdfDocument(new PdfReader(pdf1Path));
            using PdfDocument pdf2 = new PdfDocument(new PdfReader(pdf2Path));

            merger.Merge(pdf1, 1, pdf1.GetNumberOfPages());
            merger.Merge(pdf2, 1, pdf2.GetNumberOfPages());
        }
    }
}
