using MediatR;
using TAO.SharedKernel.Results;

public sealed record CreateCampaignCommand(
    Guid OrganizationId,
    string Name,
    string ReferenceNumber,
    Guid RecruiterId,
    Guid HiringManagerId,
    int NumberOfOpenings)
    : IRequest<Result<Guid>>;