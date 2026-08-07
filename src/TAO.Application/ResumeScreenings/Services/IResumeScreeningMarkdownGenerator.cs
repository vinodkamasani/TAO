using TAO.AI.ResumeScreening.Contracts;
using TAO.Domain.ValueObjects;

namespace TAO.Application.ResumeScreenings.Services;

public interface IResumeScreeningMarkdownGenerator
{
    MarkdownContent Generate(
        ResumeScreeningResult screeningResult);
}