using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TAO.AI.Abstractions;
using TAO.SharedKernel.AI.Models;
using TAO.SharedKernel.Results;
using UglyToad.PdfPig;

namespace TAO.AI.ResumeParsing.DocumentExtraction;

internal sealed class DocumentTextExtractor : IDocumentTextExtractor
{
    public Task<Result<string>> ExtractTextAsync(
        UploadedResume uploadedResume,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uploadedResume);

        var extension = Path.GetExtension(uploadedResume.FileName)
            .ToLowerInvariant();

        return extension switch
        {
            ".pdf" => ExtractPdf(uploadedResume),

            ".docx" => ExtractDocx(uploadedResume),

            ".txt" => ExtractText(uploadedResume),

            _ => Task.FromResult(
                Result<string>.Failure(
                    Error.Failure(
                        "Resume.UnsupportedFileType",
                        $"Unsupported resume format '{extension}'.")))
        };
    }

    private static Task<Result<string>> ExtractPdf(
        UploadedResume uploadedResume)
    {
        try
        {
            using var stream = new MemoryStream(uploadedResume.Content);

            using var document = PdfDocument.Open(stream);

            var text = string.Join(
                Environment.NewLine,
                document.GetPages().Select(p => p.Text));

            return Task.FromResult(
                Result<string>.Success(text));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                Result<string>.Failure(
                    Error.Failure(
                        "Resume.PdfExtractionFailed",
                        ex.Message)));
        }
    }

    private static Task<Result<string>> ExtractDocx(
        UploadedResume uploadedResume)
    {
        try
        {
            using var stream = new MemoryStream(uploadedResume.Content);

            using var document =
                WordprocessingDocument.Open(stream, false);

            var body = document.MainDocumentPart?.Document.Body;

            if (body is null)
            {
                return Task.FromResult(
                    Result<string>.Failure(
                        Error.Failure(
                            "Resume.DocxExtractionFailed",
                            "Document body not found.")));
            }

            var text = string.Join(
                Environment.NewLine,
                body.Descendants<Text>()
                    .Select(t => t.Text));

            return Task.FromResult(
                Result<string>.Success(text));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                Result<string>.Failure(
                    Error.Failure(
                        "Resume.DocxExtractionFailed",
                        ex.Message)));
        }
    }

    private static Task<Result<string>> ExtractText(
        UploadedResume uploadedResume)
    {
        var text = System.Text.Encoding.UTF8.GetString(
            uploadedResume.Content);

        return Task.FromResult(
            Result<string>.Success(text));
    }
}