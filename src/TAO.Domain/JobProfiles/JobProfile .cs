using TAO.Domain.Abstractions;
using TAO.Domain.Common;

namespace TAO.Domain.JobProfiles;

public sealed class JobProfile : Entity
{
    private JobProfile(
        Guid id,
        string title,
        string summary,
        ExperienceLevel experienceLevel,
        EmploymentType employmentType,
        WorkMode workMode,
        string location
        ) 
    {
        Title = title;
        Summary = summary;
        ExperienceLevel = experienceLevel;
        EmploymentType = employmentType;
        WorkMode = workMode;
        Location = location;
    }
    public string Title { get; private set; }

    public string Summary { get; private set; }

    public ExperienceLevel ExperienceLevel { get; private set; }

    public EmploymentType EmploymentType { get; private set; }

    public WorkMode WorkMode { get; private set; }

    public string Location { get; private set; }


    public static JobProfile Create(
        Guid id,
        string title,
        string summary,
        ExperienceLevel experienceLevel,
        EmploymentType employmentType,
        WorkMode workMode,
        string location)
    {

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));
        }

        return new JobProfile(
             id,
             title.Trim(),
             summary?.Trim() ?? string.Empty,
             experienceLevel,
             employmentType,
             workMode,
             location?.Trim() ?? string.Empty);
    }

    public void Update(
       string title,
       string summary,
       ExperienceLevel experienceLevel,
       EmploymentType employmentType,
       WorkMode workMode,
       string location)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Job title is required.",
                nameof(title));
        }

        Title = title.Trim();
        Summary = summary?.Trim() ?? string.Empty;
        ExperienceLevel = experienceLevel;
        EmploymentType = employmentType;
        WorkMode = workMode;
        Location = location?.Trim() ?? string.Empty;
    }
}
