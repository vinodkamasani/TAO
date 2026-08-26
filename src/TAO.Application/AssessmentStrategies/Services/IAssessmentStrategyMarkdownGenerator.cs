using TAO.AI.AssessmentStrategies.Contracts;
using TAO.Domain.ValueObjects;

namespace TAO.Application.AssessmentStrategies.Services;

public interface IAssessmentStrategyMarkdownGenerator
{
    MarkdownContent Generate(
        AssessmentStrategyGenerationResult generationResult);
}