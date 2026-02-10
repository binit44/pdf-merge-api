//using iText.Kernel.Pdf;
//using iText.Kernel.Utils;
//using Microsoft.AspNetCore.Mvc;
//using PdfMergeApp.Services;

//namespace PdfMergeApp.Controllers
//{
//    [ApiController]
//    [Route("api/pdf")]
//    public class PdfController : ControllerBase
//    {
//        [HttpPost("merge")]
//        public async Task<IActionResult> MergePdf(
//    IFormFile pdf1,
//    IFormFile pdf2)
//        {
//            if (pdf1 == null || pdf2 == null)
//                return BadRequest("Both PDF files are required");

//            if (!pdf1.FileName.EndsWith(".pdf") || !pdf2.FileName.EndsWith(".pdf"))
//                return BadRequest("Only PDF files allowed");

//            // Read uploaded PDFs into memory
//            using var stream1 = new MemoryStream();
//            using var stream2 = new MemoryStream();

//            await pdf1.CopyToAsync(stream1);
//            await pdf2.CopyToAsync(stream2);

//            stream1.Position = 0;
//            stream2.Position = 0;

//            using var outputStream = new MemoryStream();

//            // Merge PDFs in memory
//            using (var pdfDoc1 = new PdfDocument(new PdfReader(stream1)))
//            using (var pdfDoc2 = new PdfDocument(new PdfReader(stream2)))
//            using (var mergedPdf = new PdfDocument(new PdfWriter(outputStream)))
//            {
//                PdfMerger merger = new PdfMerger(mergedPdf);
//                merger.Merge(pdfDoc1, 1, pdfDoc1.GetNumberOfPages());
//                merger.Merge(pdfDoc2, 1, pdfDoc2.GetNumberOfPages());
//            }

//            return File(outputStream.ToArray(), "application/pdf", "Merged.pdf");
//        }
//    }
//}

//using iText.Kernel.Pdf;
//using iText.Kernel.Utils;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Mvc;
//using PdfMergeApp.Services;

//namespace PdfMergeApp.Controllers
//{
//    [ApiController]
//    [Route("api/pdf")]
//    [EnableCors("AllowAll")]
//    public class PdfController : ControllerBase
//    {
//        [HttpPost("merge")]
//        public async Task<IActionResult> MergePdf(
//            IFormFile pdf1,
//            IFormFile pdf2)
//        {
//            if (pdf1 == null || pdf2 == null)
//                return BadRequest("Both PDF files are required");

//            if (!pdf1.FileName.EndsWith(".pdf") || !pdf2.FileName.EndsWith(".pdf"))
//                return BadRequest("Only PDF files allowed");

//            // Read uploaded PDFs into memory
//            using var stream1 = new MemoryStream();
//            using var stream2 = new MemoryStream();

//            await pdf1.CopyToAsync(stream1);
//            await pdf2.CopyToAsync(stream2);

//            stream1.Position = 0;
//            stream2.Position = 0;

//            using var outputStream = new MemoryStream();

//            // Merge PDFs in memory
//            using (var pdfDoc1 = new PdfDocument(new PdfReader(stream1)))
//            using (var pdfDoc2 = new PdfDocument(new PdfReader(stream2)))
//            using (var mergedPdf = new PdfDocument(new PdfWriter(outputStream)))
//            {
//                PdfMerger merger = new PdfMerger(mergedPdf);
//                merger.Merge(pdfDoc1, 1, pdfDoc1.GetNumberOfPages());
//                merger.Merge(pdfDoc2, 1, pdfDoc2.GetNumberOfPages());
//            }

//            return File(outputStream.ToArray(), "application/pdf", "Merged.pdf");
//        }

//        // New endpoint to get page count
//        [HttpPost("pagecount")]
//        public async Task<IActionResult> GetPageCount(IFormFile pdf)
//        {
//            if (pdf == null)
//                return BadRequest("PDF file is required");

//            if (!pdf.FileName.EndsWith(".pdf"))
//                return BadRequest("Only PDF files allowed");

//            try
//            {
//                using var stream = new MemoryStream();
//                await pdf.CopyToAsync(stream);
//                stream.Position = 0;

//                using var pdfDoc = new PdfDocument(new PdfReader(stream));
//                int pageCount = pdfDoc.GetNumberOfPages();

//                return Ok(new { pageCount = pageCount });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { error = "Invalid PDF file", message = ex.Message });
//            }
//        }
//    }
//}

using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using PdfMergeApp.Services;

namespace PdfMergeApp.Controllers
{
    [ApiController]
    [Route("api/pdf")]
    [EnableCors("AllowAll")]
    public class PdfController : ControllerBase
    {
        [HttpPost("merge")]
        public async Task<IActionResult> MergePdf(
            IFormFile pdf1,
            IFormFile pdf2,
            [FromForm] string pdf1Pages = null,
            [FromForm] string pdf2Pages = null)
        {
            if (pdf1 == null || pdf2 == null)
                return BadRequest("Both PDF files are required");

            if (!pdf1.FileName.EndsWith(".pdf") || !pdf2.FileName.EndsWith(".pdf"))
                return BadRequest("Only PDF files allowed");

            try
            {
                // Read uploaded PDFs into memory
                using var stream1 = new MemoryStream();
                using var stream2 = new MemoryStream();

                await pdf1.CopyToAsync(stream1);
                await pdf2.CopyToAsync(stream2);

                stream1.Position = 0;
                stream2.Position = 0;

                using var outputStream = new MemoryStream();

                // Merge PDFs in memory
                using (var pdfDoc1 = new PdfDocument(new PdfReader(stream1)))
                using (var pdfDoc2 = new PdfDocument(new PdfReader(stream2)))
                using (var mergedPdf = new PdfDocument(new PdfWriter(outputStream)))
                {
                    PdfMerger merger = new PdfMerger(mergedPdf);

                    // Parse and merge PDF1 pages
                    var pdf1PageList = ParsePageSelection(pdf1Pages, pdfDoc1.GetNumberOfPages());
                    foreach (var page in pdf1PageList)
                    {
                        merger.Merge(pdfDoc1, page, page);
                    }

                    // Parse and merge PDF2 pages
                    var pdf2PageList = ParsePageSelection(pdf2Pages, pdfDoc2.GetNumberOfPages());
                    foreach (var page in pdf2PageList)
                    {
                        merger.Merge(pdfDoc2, page, page);
                    }
                }

                return File(outputStream.ToArray(), "application/pdf", "Merged.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error merging PDFs", message = ex.Message });
            }
        }

        private List<int> ParsePageSelection(string pageSelection, int totalPages)
        {
            var result = new List<int>();

            // If null or empty, include all pages
            if (string.IsNullOrWhiteSpace(pageSelection))
            {
                for (int i = 1; i <= totalPages; i++)
                {
                    result.Add(i);
                }
                return result;
            }

            // Parse selection like "1,3,5-7,10"
            var parts = pageSelection.Split(',');
            foreach (var part in parts)
            {
                var trimmed = part.Trim();

                if (trimmed.Contains('-'))
                {
                    // Range like "5-7"
                    var range = trimmed.Split('-');
                    if (range.Length == 2 &&
                        int.TryParse(range[0].Trim(), out int start) &&
                        int.TryParse(range[1].Trim(), out int end))
                    {
                        for (int i = start; i <= end && i <= totalPages; i++)
                        {
                            if (i > 0 && !result.Contains(i))
                                result.Add(i);
                        }
                    }
                }
                else
                {
                    // Single page like "3"
                    if (int.TryParse(trimmed, out int page))
                    {
                        if (page > 0 && page <= totalPages && !result.Contains(page))
                            result.Add(page);
                    }
                }
            }

            // Sort pages
            result.Sort();

            // If no valid pages, include all
            if (result.Count == 0)
            {
                for (int i = 1; i <= totalPages; i++)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        [HttpPost("pagecount")]
        public async Task<IActionResult> GetPageCount(IFormFile pdf)
        {
            if (pdf == null)
                return BadRequest(new { error = "PDF file is required" });

            if (!pdf.FileName.EndsWith(".pdf"))
                return BadRequest(new { error = "Only PDF files allowed" });

            try
            {
                using var stream = new MemoryStream();
                await pdf.CopyToAsync(stream);
                stream.Position = 0;

                using var pdfDoc = new PdfDocument(new PdfReader(stream));
                int pageCount = pdfDoc.GetNumberOfPages();

                return Ok(new { pageCount = pageCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Invalid PDF file", message = ex.Message });
            }
        }
    }
}