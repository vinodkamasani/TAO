using TAO.Api.Endpoints.AssessmentStrategies.Approve;
using TAO.Api.Endpoints.AssessmentStrategies.Create;
using TAO.Api.Endpoints.Campaigns.Create;
using TAO.Api.Endpoints.HiringStrategies.Approve;
using TAO.Api.Endpoints.HiringStrategies.Create;
using TAO.Api.Endpoints.HiringStrategies.Get;
using TAO.Api.Endpoints.JobProfiles.Create;
using TAO.Api.Endpoints.ResumeImports.Create;
using TAO.Api.Endpoints.ResumeScreenings.Create;

namespace TAO.Api.Endpoints.Campaigns;

public static class CampaignEndpoints
{
    public static RouteGroupBuilder MapCampaignEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/campaigns")
            .WithTags("Campaigns");

        group.MapCreateCampaignEndpoint();
        group.MapCreateJobProfileEndpoint();
        group.MapCreateHiringStrategyEndpoint();
        group.MapGetHiringStrategyEndpoint();
        group.MapApproveHiringStrategyEndpoint();
        group.MapCreateResumeImportEndpoint();
        group.MapCreateResumeScreeningEndpoint();
        group.MapCreateAssessmentStrategyEndpoint();
        group.MapApproveAssessmentStrategyEndpoint();
        return group;
    }
}