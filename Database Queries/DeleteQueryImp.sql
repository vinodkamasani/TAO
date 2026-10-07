BEGIN TRANSACTION;

BEGIN TRY

    -- 1. Clear references from AssessmentSessions
    UPDATE s
    SET
        CurrentQuestionId = NULL,
        CurrentSessionRoundId = NULL
    FROM dbo.AssessmentSessions s
    WHERE s.CreatedOn >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE))
      AND s.CreatedOn < DATEADD(DAY, 1, CAST(GETDATE() AS DATE));


    -- 2. Delete AssessmentQuestions
    DELETE q
    FROM dbo.AssessmentQuestions q
    INNER JOIN dbo.AssessmentSessionRounds r
        ON r.Id = q.AssessmentSessionRoundId
    INNER JOIN dbo.AssessmentSessions s
        ON s.Id = r.AssessmentSessionId
    WHERE s.CreatedOn >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE))
      AND s.CreatedOn < DATEADD(DAY, 1, CAST(GETDATE() AS DATE));


    -- 3. Delete AssessmentSessionRounds
    DELETE r
    FROM dbo.AssessmentSessionRounds r
    INNER JOIN dbo.AssessmentSessions s
        ON s.Id = r.AssessmentSessionId
    WHERE s.CreatedOn >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE))
      AND s.CreatedOn < DATEADD(DAY, 1, CAST(GETDATE() AS DATE));


    -- 4. Delete AssessmentSessions
    DELETE FROM dbo.AssessmentSessions
    WHERE CreatedOn >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE))
      AND CreatedOn < DATEADD(DAY, 1, CAST(GETDATE() AS DATE));

       -- 5. Delete AssessmentQuestionEvaluations
    DELETE FROM dbo.AssessmentQuestionEvaluations
    WHERE CreatedOn >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE))
      AND CreatedOn < DATEADD(DAY, 1, CAST(GETDATE() AS DATE));

       -- 5. Delete AssessmentQuestionEvaluations
    DELETE FROM dbo.AssessmentRoundEvaluations
    WHERE CreatedOn >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE))
      AND CreatedOn < DATEADD(DAY, 1, CAST(GETDATE() AS DATE));

             -- 7. Delete AssessmentQuestionEvaluations
    DELETE FROM dbo.AssessmentResults
    WHERE CreatedOn >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE))
      AND CreatedOn < DATEADD(DAY, 1, CAST(GETDATE() AS DATE));

                   -- 7. Delete AssessmentCompetencyEvaluations
    DELETE FROM dbo.AssessmentCompetencyEvaluations
    WHERE CreatedOn >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE))
      AND CreatedOn < DATEADD(DAY, 1, CAST(GETDATE() AS DATE));

    COMMIT TRANSACTION;

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;

END CATCH;


  delete from dbo.CandidateApplications where CampaignId='01A0C92B-D8F5-7778-9F75-07DA187E30AA';
     delete from dbo.Resumes
     delete from dbo.ResumeProfiles
     delete from dbo.ResumeScreenings;
     delete from dbo.[CandidateInvitations]
     delete from dbo.EmailDeliveries
      delete from dbo.AssessmentQuestionEvaluations
     delete from dbo.AssessmentQuestions
     delete from dbo.AssessmentRoundEvaluations
      delete from dbo.AssessmentSessionRounds
      delete from dbo.AssessmentCompetencyEvaluations
      delete from dbo.AssessmentResults
       delete from dbo.AssessmentSessions
       update dbo.AssessmentSessions set CurrentQuestionId=null,CurrentSessionRoundId=null;