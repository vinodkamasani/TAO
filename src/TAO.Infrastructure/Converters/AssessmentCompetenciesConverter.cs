using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TAO.Domain.ValueObjects;

namespace TAO.Infrastructure.Persistence.Converters;

public sealed class AssessmentCompetenciesConverter
    : ValueConverter<IReadOnlyCollection<AssessmentCompetency>, string>
{
    public AssessmentCompetenciesConverter()
        : base(
            competencies => JsonSerializer.Serialize(
                competencies,
                (JsonSerializerOptions?)null),

            value => JsonSerializer.Deserialize<List<AssessmentCompetency>>(
                value,
                (JsonSerializerOptions?)null)
                ?? new List<AssessmentCompetency>())
    {
    }
}