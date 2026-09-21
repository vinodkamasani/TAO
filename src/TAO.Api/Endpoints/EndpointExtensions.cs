using TAO.Api.Endpoints.AssessmentQuestionEvaluations.Evaluate;
using TAO.Api.Endpoints.AssessmentQuestions;
using TAO.Api.Endpoints.AssessmentQuestions.CandidateResponse;
using TAO.Api.Endpoints.AssessmentQuestions.CodeResponse;
using TAO.Api.Endpoints.AssessmentQuestions.Complete;
using TAO.Api.Endpoints.AssessmentQuestions.FollowUp;
using TAO.Api.Endpoints.AssessmentQuestions.Skip;
using TAO.Api.Endpoints.AssessmentSessions;
using TAO.Api.Endpoints.Auth;
using TAO.Api.Endpoints.Campaigns;
using TAO.Api.Endpoints.Users;


namespace TAO.Api;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapOrganizationEndpoints();

        app.MapCampaignEndpoints();
        app.MapJobProfileEndpoints();
        app.MapStaticAssets();
        app.MapAssessmentSessionEndpoints();
        app.MapStartAssessmentSessionEndpoint();
        app.MapGenerateAssessmentQuestionEndpoint();
        app.MapRecordCandidateResponseEndpoint();
        app.MapRecordCodeResponseEndpoint();
        app.MapGenerateFollowUpEndpoint();
        app.MapCompleteAssessmentQuestionEndpoint();
        app.MapSkipAssessmentQuestionEndpoint();
        app.MapEvaluateAssessmentQuestionEndpoint();
        app.MapCandidateApplicationEndpoints();
        app.MapUserEndpoints();
        app.MapAuthEndpoints();
        return app;
    }
}