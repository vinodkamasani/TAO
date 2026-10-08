using TAO.Api.Endpoints.AssessmentQuestionEvaluations.Evaluate;
using TAO.Api.Endpoints.AssessmentQuestions;
using TAO.Api.Endpoints.AssessmentQuestions.CandidateResponse;
using TAO.Api.Endpoints.AssessmentQuestions.CodeResponse;
using TAO.Api.Endpoints.AssessmentQuestions.Complete;
using TAO.Api.Endpoints.AssessmentQuestions.FollowUp;
using TAO.Api.Endpoints.AssessmentQuestions.Skip;
using TAO.Api.Endpoints.AssessmentResults;
using TAO.Api.Endpoints.AssessmentResults.GetAssessmentQuestion;
using TAO.Api.Endpoints.AssessmentResults.GetAssessmentQuestionCode;
using TAO.Api.Endpoints.AssessmentResults.GetAssessmentQuestionConversation;
using TAO.Api.Endpoints.AssessmentResults.GetAssessmentRound;
using TAO.Api.Endpoints.AssessmentSessions;
using TAO.Api.Endpoints.AssessmentSessions.Advance;
using TAO.Api.Endpoints.AssessmentSessions.Workflow;
using TAO.Api.Endpoints.Auth;
using TAO.Api.Endpoints.Campaigns;
using TAO.Api.Endpoints.Candidates.Assessment;
using TAO.Api.Endpoints.Candidates.Signup;
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
        app.MapGetAssessmentWorkflowEndpoint();
        app.MapCandidateSignupEndpoint();
        app.MapAdvanceAssessmentSessionEndpoint();
        app.MapGetCandidateAssessmentContextEndpoint();
        app.MapGetAssessmentSummaryEndpoint();
        app.MapGetAssessmentRoundEndpoint();
        app.MapGetAssessmentQuestionEndpoint();
        app.MapGetAssessmentQuestionConversationEndpoint();
        app.MapGetAssessmentQuestionCodeEndpoint();
        return app;
    }
}