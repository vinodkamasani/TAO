namespace TAO.SharedKernel.AI.Models;

public sealed class UploadedResume
{
    public string FileName { get; }
    public string ContentType { get; }
    public byte[] Content { get; }

    public UploadedResume(string fileName, string contentType, byte[] content)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name must not be null or whitespace.", nameof(fileName));
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type must not be null or whitespace.", nameof(contentType));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        FileName = fileName;
        ContentType = contentType;
    }

}