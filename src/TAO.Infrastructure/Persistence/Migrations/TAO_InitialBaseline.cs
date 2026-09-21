using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAO.Infrastructure.Persistence.Migrations;

/// <summary>
/// Baseline migration generated from DB_Create(2).sql.
///
/// The supplied SQL database design is treated as the source of truth.
/// ASP.NET Core Identity tables are included separately.
/// This migration intentionally does not include an EF Designer/Snapshot.
/// </summary>
public partial class InitialBaseline : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Organizations",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                Name = table.Column<string>(
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: false),
                Code = table.Column<string>(
                    type: "nvarchar(50)",
                    maxLength: 50,
                    nullable: false),
                Status = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Organizations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OrganizationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                FirstName = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: false),
                LastName = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: false),
                Email = table.Column<string>(
                    type: "nvarchar(256)",
                    maxLength: 256,
                    nullable: false),
                Role = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                Status = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Campaigns",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OrganizationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                Name = table.Column<string>(
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: false),
                ReferenceNumber = table.Column<string>(
                    type: "nvarchar(50)",
                    maxLength: 50,
                    nullable: false),
                RecruiterId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                HiringManagerId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                NumberOfOpenings = table.Column<int>(
                    type: "int",
                    nullable: false),
                Status = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Campaigns", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CandidateApplications",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OrganizationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                CampaignId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                CandidateName = table.Column<string>(
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: false),
                Email = table.Column<string>(
                    type: "nvarchar(320)",
                    maxLength: 320,
                    nullable: false),
                Phone = table.Column<string>(
                    type: "nvarchar(20)",
                    maxLength: 20,
                    nullable: false),
                LinkedInUrl = table.Column<string>(
                    type: "nvarchar(500)",
                    maxLength: 500,
                    nullable: true),
                CurrentCompany = table.Column<string>(
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: true),
                CurrentLocation = table.Column<string>(
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: true),
                OverallMatchPercentage = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                IsRecommended = table.Column<bool>(
                    type: "bit",
                    nullable: false),
                ResumeUploadedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                LastScreenedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CandidateApplications", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "JobProfiles",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OrganizationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                CampaignId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OriginalJobDescription = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                Prompt = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                RawResponse = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                ProviderName = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: false),
                ModelName = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: false),
                PromptVersion = table.Column<int>(
                    type: "int",
                    nullable: false),
                GeneratedContent = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                StructuredProfile = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                Status = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                GeneratedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ApprovedByUserId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ApprovedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_JobProfiles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "HiringStrategies",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OrganizationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                CampaignId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                GeneratedContent = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                StructuredData = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                Status = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                ApprovedByUserId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ApprovedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                Prompt = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                RawResponse = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                ProviderName = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: false),
                ModelName = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: false),
                PromptVersion = table.Column<int>(
                    type: "int",
                    nullable: false),
                GeneratedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HiringStrategies", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AssessmentStrategies",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OrganizationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                CampaignId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                AssessmentName = table.Column<string>(
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: false),
                GeneratedContent = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                StructuredData = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                Status = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                GeneratedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ApprovedByUserId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ApprovedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                Prompt = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                RawResponse = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                ProviderName = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: false),
                ModelName = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: false),
                PromptVersion = table.Column<int>(
                    type: "int",
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssessmentStrategies", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AssessmentRounds",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                AssessmentStrategyId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                Order = table.Column<int>(
                    type: "int",
                    nullable: false),
                Type = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                Difficulty = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                DurationInMinutes = table.Column<int>(
                    type: "int",
                    nullable: false),
                TargetQuestionCount = table.Column<int>(
                    type: "int",
                    nullable: false),
                Competencies = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssessmentRounds", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ResumeImports",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OrganizationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                CampaignId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                Status = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                TotalFiles = table.Column<int>(
                    type: "int",
                    nullable: false),
                SuccessfulFiles = table.Column<int>(
                    type: "int",
                    nullable: false),
                FailedFiles = table.Column<int>(
                    type: "int",
                    nullable: false),
                CompletedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ResumeImports", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ResumeImportFailures",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OrganizationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                ResumeImportId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                FileName = table.Column<string>(
                    type: "nvarchar(255)",
                    maxLength: 255,
                    nullable: false),
                FailureReason = table.Column<string>(
                    type: "nvarchar(500)",
                    maxLength: 500,
                    nullable: false),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ResumeImportFailures", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Resumes",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OrganizationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                ApplicationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                FileName = table.Column<string>(
                    type: "nvarchar(255)",
                    maxLength: 255,
                    nullable: false),
                ContentType = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: false),
                FileSize = table.Column<long>(
                    type: "bigint",
                    nullable: false),
                FileContent = table.Column<byte[]>(
                    type: "varbinary(max)",
                    nullable: false),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Resumes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ResumeProfiles",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OrganizationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                ApplicationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                StructuredContent = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                GeneratedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ResumeProfiles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ResumeScreenings",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OrganizationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                ApplicationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                Content = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                StructuredContent = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                GeneratedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ResumeScreenings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "EmailDeliveries",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OrganizationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                CampaignId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                CandidateApplicationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                RecipientEmail = table.Column<string>(
                    type: "nvarchar(320)",
                    maxLength: 320,
                    nullable: false),
                Subject = table.Column<string>(
                    type: "nvarchar(500)",
                    maxLength: 500,
                    nullable: false),
                Body = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                Status = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                SentOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                FailedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                FailureReason = table.Column<string>(
                    type: "nvarchar(2000)",
                    maxLength: 2000,
                    nullable: true),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EmailDeliveries", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AssessmentSessions",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                CandidateApplicationId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                AssessmentStrategyId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                StrategySnapshot = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                Status = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                CurrentSessionRoundId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CurrentQuestionId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ConsentAcceptedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                ConsentVersion = table.Column<int>(
                    type: "int",
                    nullable: false),
                StartedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                CompletedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                AssessmentExpiresOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                LastActivityOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                HasUsedInterruptionWindow = table.Column<bool>(
                    type: "bit",
                    nullable: false),
                IsInterrupted = table.Column<bool>(
                    type: "bit",
                    nullable: false),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false,
                    defaultValueSql: "sysutcdatetime()"),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssessmentSessions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AssessmentSessionRounds",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                AssessmentSessionId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                AssessmentRoundId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                Order = table.Column<int>(
                    type: "int",
                    nullable: false),
                Type = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                Difficulty = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                DurationInMinutes = table.Column<int>(
                    type: "int",
                    nullable: false),
                TargetQuestionCount = table.Column<int>(
                    type: "int",
                    nullable: false),
                Competencies = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                Status = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                StartedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                ExpiresOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                CompletedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false,
                    defaultValueSql: "sysutcdatetime()"),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssessmentSessionRounds", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AssessmentQuestions",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                AssessmentSessionRoundId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                Order = table.Column<int>(
                    type: "int",
                    nullable: false),
                PrimaryQuestion = table.Column<string>(
                    type: "nvarchar(4000)",
                    maxLength: 4000,
                    nullable: false),
                Status = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                Conversation = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: true),
                CandidateCode = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: true),
                StartedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                CompletedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                Competencies = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                CreatedBy = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false,
                    defaultValueSql: "sysutcdatetime()"),
                ModifiedBy = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssessmentQuestions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AssessmentResults",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                AssessmentSessionId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                OverallScore = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                OverallConfidence = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                Recommendation = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                ExecutiveSummary = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                GeneratedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ReviewedByUserId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ReviewedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                RecruiterDecision = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: true),
                RecruiterComments = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: true),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssessmentResults", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AssessmentRoundEvaluations",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                AssessmentSessionRoundId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                Score = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                Confidence = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                Strengths = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                Gaps = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                Evidence = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false,
                    defaultValueSql: "sysutcdatetime()"),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssessmentRoundEvaluations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AssessmentQuestionEvaluations",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                AssessmentQuestionId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                Score = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                Confidence = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                Strengths = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                Gaps = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                Evidence = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: false),
                Competencies = table.Column<string>(
                    type: "nvarchar(max)",
                    nullable: true),
                CreatedBy = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssessmentQuestionEvaluations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AssessmentCompetencyEvaluations",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                AssessmentResultId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),
                CompetencyName = table.Column<string>(
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: false),
                Priority = table.Column<string>(
                    type: "nvarchar(50)",
                    maxLength: 50,
                    nullable: false),
                Score = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                MinimumPassPercentage = table.Column<byte>(
                    type: "tinyint",
                    nullable: false),
                CreatedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                CreatedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: false),
                ModifiedBy = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: true),
                ModifiedOn = table.Column<DateTime>(
                    type: "datetime2(7)",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssessmentCompetencyEvaluations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "IdentityUsers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IdentityUsers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "IdentityRoles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IdentityRoles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "IdentityUserClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IdentityUserClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_IdentityUserClaims_IdentityUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "IdentityUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "IdentityRoleClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IdentityRoleClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_IdentityRoleClaims_IdentityRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "IdentityRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "IdentityUserLogins",
            columns: table => new
            {
                LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IdentityUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_IdentityUserLogins_IdentityUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "IdentityUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "IdentityUserRoles",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IdentityUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_IdentityUserRoles_IdentityUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "IdentityUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_IdentityUserRoles_IdentityRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "IdentityRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "IdentityUserTokens",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IdentityUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_IdentityUserTokens_IdentityUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "IdentityUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "UX_AssessmentCompetencyEvaluations_Result_Competency",
            table: "AssessmentCompetencyEvaluations",
            columns: new[]
            {
                "AssessmentResultId",
                "CompetencyName"
            },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UX_AssessmentQuestionEvaluations_QuestionId",
            table: "AssessmentQuestionEvaluations",
            column: "AssessmentQuestionId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UX_AssessmentQuestions_SessionRound_Order",
            table: "AssessmentQuestions",
            columns: new[]
            {
                "AssessmentSessionRoundId",
                "Order"
            },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AssessmentResults_ReviewedByUserId",
            table: "AssessmentResults",
            column: "ReviewedByUserId"
);

        migrationBuilder.CreateIndex(
            name: "UX_AssessmentResults_AssessmentSessionId",
            table: "AssessmentResults",
            column: "AssessmentSessionId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UX_AssessmentRoundEvaluations_SessionRoundId",
            table: "AssessmentRoundEvaluations",
            column: "AssessmentSessionRoundId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AssessmentRounds_AssessmentStrategyId_Order",
            table: "AssessmentRounds",
            columns: new[]
            {
                "AssessmentStrategyId",
                "Order"
            },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AssessmentSessionRounds_AssessmentRoundId",
            table: "AssessmentSessionRounds",
            column: "AssessmentRoundId"
);

        migrationBuilder.CreateIndex(
            name: "UX_AssessmentSessionRounds_Session_Order",
            table: "AssessmentSessionRounds",
            columns: new[]
            {
                "AssessmentSessionId",
                "Order"
            },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AssessmentSessions_AssessmentStrategyId",
            table: "AssessmentSessions",
            column: "AssessmentStrategyId"
);

        migrationBuilder.CreateIndex(
            name: "IX_AssessmentSessions_CandidateApplicationId",
            table: "AssessmentSessions",
            column: "CandidateApplicationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_AssessmentSessions_Status_Expires",
            table: "AssessmentSessions",
            columns: new[]
            {
                "Status",
                "AssessmentExpiresOn"
            }
);

        migrationBuilder.CreateIndex(
            name: "IX_AssessmentStrategies_ApprovedByUserId",
            table: "AssessmentStrategies",
            column: "ApprovedByUserId"
);

        migrationBuilder.CreateIndex(
            name: "IX_AssessmentStrategies_CampaignId",
            table: "AssessmentStrategies",
            column: "CampaignId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AssessmentStrategies_OrganizationId",
            table: "AssessmentStrategies",
            column: "OrganizationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_Campaigns_HiringManagerId",
            table: "Campaigns",
            column: "HiringManagerId"
);

        migrationBuilder.CreateIndex(
            name: "IX_Campaigns_OrganizationId",
            table: "Campaigns",
            column: "OrganizationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_Campaigns_OrganizationId_ReferenceNumber",
            table: "Campaigns",
            columns: new[]
            {
                "OrganizationId",
                "ReferenceNumber"
            },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Campaigns_OrganizationId_Status",
            table: "Campaigns",
            columns: new[]
            {
                "OrganizationId",
                "Status"
            }
);

        migrationBuilder.CreateIndex(
            name: "IX_Campaigns_RecruiterId",
            table: "Campaigns",
            column: "RecruiterId"
);

        migrationBuilder.CreateIndex(
            name: "IX_Campaigns_Status",
            table: "Campaigns",
            column: "Status"
);

        migrationBuilder.CreateIndex(
            name: "IX_CandidateApplications_CampaignId",
            table: "CandidateApplications",
            column: "CampaignId"
);

        migrationBuilder.CreateIndex(
            name: "IX_CandidateApplications_Email",
            table: "CandidateApplications",
            column: "Email"
);

        migrationBuilder.CreateIndex(
            name: "IX_CandidateApplications_OrganizationId",
            table: "CandidateApplications",
            column: "OrganizationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_CandidateApplications_OrganizationId_CampaignId",
            table: "CandidateApplications",
            columns: new[]
            {
                "OrganizationId",
                "CampaignId"
            }
);

        migrationBuilder.CreateIndex(
            name: "IX_CandidateApplications_OrganizationId_CampaignId_IsRecommended",
            table: "CandidateApplications",
            columns: new[]
            {
                "OrganizationId",
                "CampaignId",
                "IsRecommended"
            }
);

        migrationBuilder.CreateIndex(
            name: "IX_CandidateApplications_OrganizationId_CampaignId_OverallMatchPercentage",
            table: "CandidateApplications",
            columns: new[]
            {
                "OrganizationId",
                "CampaignId",
                "OverallMatchPercentage"
            }
);

        migrationBuilder.CreateIndex(
            name: "IX_EmailDeliveries_CampaignId_CandidateApplicationId",
            table: "EmailDeliveries",
            columns: new[]
            {
                "CampaignId",
                "CandidateApplicationId"
            },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_EmailDeliveries_RecipientEmail",
            table: "EmailDeliveries",
            column: "RecipientEmail"
);

        migrationBuilder.CreateIndex(
            name: "IX_HiringStrategies_ApprovedByUserId",
            table: "HiringStrategies",
            column: "ApprovedByUserId"
);

        migrationBuilder.CreateIndex(
            name: "IX_HiringStrategies_CampaignId",
            table: "HiringStrategies",
            column: "CampaignId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_HiringStrategies_OrganizationId",
            table: "HiringStrategies",
            column: "OrganizationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_JobProfiles_ApprovedByUserId",
            table: "JobProfiles",
            column: "ApprovedByUserId"
);

        migrationBuilder.CreateIndex(
            name: "IX_JobProfiles_CampaignId",
            table: "JobProfiles",
            column: "CampaignId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_JobProfiles_OrganizationId",
            table: "JobProfiles",
            column: "OrganizationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_Organizations_Code",
            table: "Organizations",
            column: "Code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Organizations_Name",
            table: "Organizations",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Organizations_Status",
            table: "Organizations",
            column: "Status"
);

        migrationBuilder.CreateIndex(
            name: "IX_ResumeImportFailures_OrganizationId",
            table: "ResumeImportFailures",
            column: "OrganizationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_ResumeImportFailures_ResumeImportId",
            table: "ResumeImportFailures",
            column: "ResumeImportId"
);

        migrationBuilder.CreateIndex(
            name: "IX_ResumeImports_CampaignId",
            table: "ResumeImports",
            column: "CampaignId"
);

        migrationBuilder.CreateIndex(
            name: "IX_ResumeImports_OrganizationId",
            table: "ResumeImports",
            column: "OrganizationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_ResumeProfiles_ApplicationId",
            table: "ResumeProfiles",
            column: "ApplicationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_ResumeProfiles_OrganizationId",
            table: "ResumeProfiles",
            column: "OrganizationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_ResumeProfiles_OrganizationId_ApplicationId",
            table: "ResumeProfiles",
            columns: new[]
            {
                "OrganizationId",
                "ApplicationId"
            }
);

        migrationBuilder.CreateIndex(
            name: "IX_Resumes_ApplicationId",
            table: "Resumes",
            column: "ApplicationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_Resumes_OrganizationId",
            table: "Resumes",
            column: "OrganizationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_Resumes_OrganizationId_ApplicationId",
            table: "Resumes",
            columns: new[]
            {
                "OrganizationId",
                "ApplicationId"
            }
);

        migrationBuilder.CreateIndex(
            name: "IX_ResumeScreenings_ApplicationId",
            table: "ResumeScreenings",
            column: "ApplicationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_ResumeScreenings_OrganizationId",
            table: "ResumeScreenings",
            column: "OrganizationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_ResumeScreenings_OrganizationId_ApplicationId",
            table: "ResumeScreenings",
            columns: new[]
            {
                "OrganizationId",
                "ApplicationId"
            }
);

        migrationBuilder.CreateIndex(
            name: "IX_Users_OrganizationId",
            table: "Users",
            column: "OrganizationId"
);

        migrationBuilder.CreateIndex(
            name: "IX_Users_OrganizationId_Email",
            table: "Users",
            columns: new[]
            {
                "OrganizationId",
                "Email"
            },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            table: "IdentityUsers",
            column: "NormalizedUserName",
            unique: true,
            filter: "[NormalizedUserName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "IdentityUsers",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            table: "IdentityRoles",
            column: "NormalizedName",
            unique: true,
            filter: "[NormalizedName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_IdentityUserClaims_UserId",
            table: "IdentityUserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_IdentityRoleClaims_RoleId",
            table: "IdentityRoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "IX_IdentityUserLogins_UserId",
            table: "IdentityUserLogins",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_IdentityUserRoles_RoleId",
            table: "IdentityUserRoles",
            column: "RoleId");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentCompetencyEvaluations_AssessmentResults",
            table: "AssessmentCompetencyEvaluations",
            column: "AssessmentResultId",
            principalTable: "AssessmentResults",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentQuestionEvaluations_AssessmentQuestions",
            table: "AssessmentQuestionEvaluations",
            column: "AssessmentQuestionId",
            principalTable: "AssessmentQuestions",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentQuestions_AssessmentSessionRounds",
            table: "AssessmentQuestions",
            column: "AssessmentSessionRoundId",
            principalTable: "AssessmentSessionRounds",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentResults_AssessmentSessions",
            table: "AssessmentResults",
            column: "AssessmentSessionId",
            principalTable: "AssessmentSessions",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentResults_ReviewedByUser",
            table: "AssessmentResults",
            column: "ReviewedByUserId",
            principalTable: "Users",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentRoundEvaluations_AssessmentSessionRounds",
            table: "AssessmentRoundEvaluations",
            column: "AssessmentSessionRoundId",
            principalTable: "AssessmentSessionRounds",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentRounds_AssessmentStrategies_AssessmentStrategyId",
            table: "AssessmentRounds",
            column: "AssessmentStrategyId",
            principalTable: "AssessmentStrategies",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentSessionRounds_AssessmentRounds",
            table: "AssessmentSessionRounds",
            column: "AssessmentRoundId",
            principalTable: "AssessmentRounds",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentSessionRounds_AssessmentSessions",
            table: "AssessmentSessionRounds",
            column: "AssessmentSessionId",
            principalTable: "AssessmentSessions",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentSessions_AssessmentStrategies",
            table: "AssessmentSessions",
            column: "AssessmentStrategyId",
            principalTable: "AssessmentStrategies",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentSessions_CandidateApplications",
            table: "AssessmentSessions",
            column: "CandidateApplicationId",
            principalTable: "CandidateApplications",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentSessions_CurrentQuestion",
            table: "AssessmentSessions",
            column: "CurrentQuestionId",
            principalTable: "AssessmentQuestions",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentSessions_CurrentSessionRound",
            table: "AssessmentSessions",
            column: "CurrentSessionRoundId",
            principalTable: "AssessmentSessionRounds",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentStrategies_Campaigns_CampaignId",
            table: "AssessmentStrategies",
            column: "CampaignId",
            principalTable: "Campaigns",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentStrategies_Organizations_OrganizationId",
            table: "AssessmentStrategies",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_AssessmentStrategies_Users_ApprovedByUserId",
            table: "AssessmentStrategies",
            column: "ApprovedByUserId",
            principalTable: "Users",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Campaigns_Organizations_OrganizationId",
            table: "Campaigns",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Campaigns_Users_HiringManagerId",
            table: "Campaigns",
            column: "HiringManagerId",
            principalTable: "Users",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Campaigns_Users_RecruiterId",
            table: "Campaigns",
            column: "RecruiterId",
            principalTable: "Users",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_CandidateApplications_Campaigns_CampaignId",
            table: "CandidateApplications",
            column: "CampaignId",
            principalTable: "Campaigns",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_CandidateApplications_Organizations_OrganizationId",
            table: "CandidateApplications",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_EmailDeliveries_Campaigns_CampaignId",
            table: "EmailDeliveries",
            column: "CampaignId",
            principalTable: "Campaigns",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_EmailDeliveries_CandidateApplications_CandidateApplicationId",
            table: "EmailDeliveries",
            column: "CandidateApplicationId",
            principalTable: "CandidateApplications",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_EmailDeliveries_Organizations_OrganizationId",
            table: "EmailDeliveries",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_HiringStrategies_Campaigns_CampaignId",
            table: "HiringStrategies",
            column: "CampaignId",
            principalTable: "Campaigns",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_HiringStrategies_Organizations_OrganizationId",
            table: "HiringStrategies",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_HiringStrategies_Users_ApprovedByUserId",
            table: "HiringStrategies",
            column: "ApprovedByUserId",
            principalTable: "Users",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_JobProfiles_Campaigns_CampaignId",
            table: "JobProfiles",
            column: "CampaignId",
            principalTable: "Campaigns",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_JobProfiles_Organizations_OrganizationId",
            table: "JobProfiles",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_JobProfiles_Users_ApprovedByUserId",
            table: "JobProfiles",
            column: "ApprovedByUserId",
            principalTable: "Users",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ResumeImportFailures_Organizations_OrganizationId",
            table: "ResumeImportFailures",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ResumeImportFailures_ResumeImports_ResumeImportId",
            table: "ResumeImportFailures",
            column: "ResumeImportId",
            principalTable: "ResumeImports",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ResumeImports_Campaigns_CampaignId",
            table: "ResumeImports",
            column: "CampaignId",
            principalTable: "Campaigns",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ResumeImports_Organizations_OrganizationId",
            table: "ResumeImports",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ResumeProfiles_CandidateApplications_ApplicationId",
            table: "ResumeProfiles",
            column: "ApplicationId",
            principalTable: "CandidateApplications",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ResumeProfiles_Organizations_OrganizationId",
            table: "ResumeProfiles",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Resumes_CandidateApplications_ApplicationId",
            table: "Resumes",
            column: "ApplicationId",
            principalTable: "CandidateApplications",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Resumes_Organizations_OrganizationId",
            table: "Resumes",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ResumeScreenings_CandidateApplications_ApplicationId",
            table: "ResumeScreenings",
            column: "ApplicationId",
            principalTable: "CandidateApplications",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ResumeScreenings_Organizations_OrganizationId",
            table: "ResumeScreenings",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Users_Organizations_OrganizationId",
            table: "Users",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id");

    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Drop FKs first.
        migrationBuilder.DropForeignKey(
            name: "FK_Users_Organizations_OrganizationId",
            table: "Users");
        migrationBuilder.DropForeignKey(
            name: "FK_ResumeScreenings_Organizations_OrganizationId",
            table: "ResumeScreenings");
        migrationBuilder.DropForeignKey(
            name: "FK_ResumeScreenings_CandidateApplications_ApplicationId",
            table: "ResumeScreenings");
        migrationBuilder.DropForeignKey(
            name: "FK_Resumes_Organizations_OrganizationId",
            table: "Resumes");
        migrationBuilder.DropForeignKey(
            name: "FK_Resumes_CandidateApplications_ApplicationId",
            table: "Resumes");
        migrationBuilder.DropForeignKey(
            name: "FK_ResumeProfiles_Organizations_OrganizationId",
            table: "ResumeProfiles");
        migrationBuilder.DropForeignKey(
            name: "FK_ResumeProfiles_CandidateApplications_ApplicationId",
            table: "ResumeProfiles");
        migrationBuilder.DropForeignKey(
            name: "FK_ResumeImports_Organizations_OrganizationId",
            table: "ResumeImports");
        migrationBuilder.DropForeignKey(
            name: "FK_ResumeImports_Campaigns_CampaignId",
            table: "ResumeImports");
        migrationBuilder.DropForeignKey(
            name: "FK_ResumeImportFailures_ResumeImports_ResumeImportId",
            table: "ResumeImportFailures");
        migrationBuilder.DropForeignKey(
            name: "FK_ResumeImportFailures_Organizations_OrganizationId",
            table: "ResumeImportFailures");
        migrationBuilder.DropForeignKey(
            name: "FK_JobProfiles_Users_ApprovedByUserId",
            table: "JobProfiles");
        migrationBuilder.DropForeignKey(
            name: "FK_JobProfiles_Organizations_OrganizationId",
            table: "JobProfiles");
        migrationBuilder.DropForeignKey(
            name: "FK_JobProfiles_Campaigns_CampaignId",
            table: "JobProfiles");
        migrationBuilder.DropForeignKey(
            name: "FK_HiringStrategies_Users_ApprovedByUserId",
            table: "HiringStrategies");
        migrationBuilder.DropForeignKey(
            name: "FK_HiringStrategies_Organizations_OrganizationId",
            table: "HiringStrategies");
        migrationBuilder.DropForeignKey(
            name: "FK_HiringStrategies_Campaigns_CampaignId",
            table: "HiringStrategies");
        migrationBuilder.DropForeignKey(
            name: "FK_EmailDeliveries_Organizations_OrganizationId",
            table: "EmailDeliveries");
        migrationBuilder.DropForeignKey(
            name: "FK_EmailDeliveries_CandidateApplications_CandidateApplicationId",
            table: "EmailDeliveries");
        migrationBuilder.DropForeignKey(
            name: "FK_EmailDeliveries_Campaigns_CampaignId",
            table: "EmailDeliveries");
        migrationBuilder.DropForeignKey(
            name: "FK_CandidateApplications_Organizations_OrganizationId",
            table: "CandidateApplications");
        migrationBuilder.DropForeignKey(
            name: "FK_CandidateApplications_Campaigns_CampaignId",
            table: "CandidateApplications");
        migrationBuilder.DropForeignKey(
            name: "FK_Campaigns_Users_RecruiterId",
            table: "Campaigns");
        migrationBuilder.DropForeignKey(
            name: "FK_Campaigns_Users_HiringManagerId",
            table: "Campaigns");
        migrationBuilder.DropForeignKey(
            name: "FK_Campaigns_Organizations_OrganizationId",
            table: "Campaigns");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentStrategies_Users_ApprovedByUserId",
            table: "AssessmentStrategies");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentStrategies_Organizations_OrganizationId",
            table: "AssessmentStrategies");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentStrategies_Campaigns_CampaignId",
            table: "AssessmentStrategies");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentSessions_CurrentSessionRound",
            table: "AssessmentSessions");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentSessions_CurrentQuestion",
            table: "AssessmentSessions");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentSessions_CandidateApplications",
            table: "AssessmentSessions");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentSessions_AssessmentStrategies",
            table: "AssessmentSessions");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentSessionRounds_AssessmentSessions",
            table: "AssessmentSessionRounds");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentSessionRounds_AssessmentRounds",
            table: "AssessmentSessionRounds");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentRounds_AssessmentStrategies_AssessmentStrategyId",
            table: "AssessmentRounds");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentRoundEvaluations_AssessmentSessionRounds",
            table: "AssessmentRoundEvaluations");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentResults_ReviewedByUser",
            table: "AssessmentResults");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentResults_AssessmentSessions",
            table: "AssessmentResults");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentQuestions_AssessmentSessionRounds",
            table: "AssessmentQuestions");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentQuestionEvaluations_AssessmentQuestions",
            table: "AssessmentQuestionEvaluations");
        migrationBuilder.DropForeignKey(
            name: "FK_AssessmentCompetencyEvaluations_AssessmentResults",
            table: "AssessmentCompetencyEvaluations");

        // Drop Identity tables.
        migrationBuilder.DropTable(name: "IdentityUserClaims");
        migrationBuilder.DropTable(name: "IdentityRoleClaims");
        migrationBuilder.DropTable(name: "IdentityUserLogins");
        migrationBuilder.DropTable(name: "IdentityUserRoles");
        migrationBuilder.DropTable(name: "IdentityUserTokens");
        migrationBuilder.DropTable(name: "IdentityRoles");
        migrationBuilder.DropTable(name: "IdentityUsers");

        // Drop TAO tables in reverse dependency order.
        migrationBuilder.DropTable(name: "AssessmentCompetencyEvaluations");
        migrationBuilder.DropTable(name: "AssessmentQuestionEvaluations");
        migrationBuilder.DropTable(name: "AssessmentRoundEvaluations");
        migrationBuilder.DropTable(name: "AssessmentResults");
        migrationBuilder.DropTable(name: "AssessmentQuestions");
        migrationBuilder.DropTable(name: "AssessmentSessionRounds");
        migrationBuilder.DropTable(name: "AssessmentSessions");
        migrationBuilder.DropTable(name: "EmailDeliveries");
        migrationBuilder.DropTable(name: "ResumeScreenings");
        migrationBuilder.DropTable(name: "ResumeProfiles");
        migrationBuilder.DropTable(name: "Resumes");
        migrationBuilder.DropTable(name: "ResumeImportFailures");
        migrationBuilder.DropTable(name: "ResumeImports");
        migrationBuilder.DropTable(name: "AssessmentRounds");
        migrationBuilder.DropTable(name: "AssessmentStrategies");
        migrationBuilder.DropTable(name: "HiringStrategies");
        migrationBuilder.DropTable(name: "JobProfiles");
        migrationBuilder.DropTable(name: "CandidateApplications");
        migrationBuilder.DropTable(name: "Campaigns");
        migrationBuilder.DropTable(name: "Users");
        migrationBuilder.DropTable(name: "Organizations");
    }
}
