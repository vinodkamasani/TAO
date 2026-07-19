namespace TAO.Domain.Enums;

public enum ResumeImportStatus : byte
{
    Queued = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4
}