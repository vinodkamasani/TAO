using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAO.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Role = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecruiterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HiringManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberOfOpenings = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaigns_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaigns_Users_HiringManagerId",
                        column: x => x.HiringManagerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaigns_Users_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CandidateApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CurrentCompany = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OverallMatchPercentage = table.Column<byte>(type: "tinyint", nullable: false),
                    IsRecommended = table.Column<bool>(type: "bit", nullable: false),
                    ResumeUploadedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    LastScreenedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateApplications_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateApplications_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HiringStrategies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeneratedContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StructuredData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    GeneratedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HiringStrategies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HiringStrategies_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HiringStrategies_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HiringStrategies_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalJobDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StructuredData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    GeneratedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobProfiles_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobProfiles_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobProfiles_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResumeImports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    TotalFiles = table.Column<int>(type: "int", nullable: false),
                    SuccessfulFiles = table.Column<int>(type: "int", nullable: false),
                    FailedFiles = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeImports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeImports_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResumeImports_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResumeProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StructuredContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeProfiles_CandidateApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "CandidateApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResumeProfiles_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resumes_CandidateApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "CandidateApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resumes_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResumeScreenings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StructuredContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeScreenings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeScreenings_CandidateApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "CandidateApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResumeScreenings_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResumeImportFailures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumeImportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeImportFailures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeImportFailures_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResumeImportFailures_ResumeImports_ResumeImportId",
                        column: x => x.ResumeImportId,
                        principalTable: "ResumeImports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_HiringManagerId",
                table: "Campaigns",
                column: "HiringManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_OrganizationId",
                table: "Campaigns",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_OrganizationId_ReferenceNumber",
                table: "Campaigns",
                columns: new[] { "OrganizationId", "ReferenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_OrganizationId_Status",
                table: "Campaigns",
                columns: new[] { "OrganizationId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_RecruiterId",
                table: "Campaigns",
                column: "RecruiterId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_Status",
                table: "Campaigns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateApplications_CampaignId",
                table: "CandidateApplications",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateApplications_Email",
                table: "CandidateApplications",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateApplications_OrganizationId",
                table: "CandidateApplications",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateApplications_OrganizationId_CampaignId",
                table: "CandidateApplications",
                columns: new[] { "OrganizationId", "CampaignId" });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateApplications_OrganizationId_CampaignId_IsRecommended",
                table: "CandidateApplications",
                columns: new[] { "OrganizationId", "CampaignId", "IsRecommended" });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateApplications_OrganizationId_CampaignId_OverallMatchPercentage",
                table: "CandidateApplications",
                columns: new[] { "OrganizationId", "CampaignId", "OverallMatchPercentage" });

            migrationBuilder.CreateIndex(
                name: "IX_HiringStrategies_ApprovedByUserId",
                table: "HiringStrategies",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HiringStrategies_CampaignId",
                table: "HiringStrategies",
                column: "CampaignId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HiringStrategies_OrganizationId",
                table: "HiringStrategies",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_JobProfiles_ApprovedByUserId",
                table: "JobProfiles",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobProfiles_CampaignId",
                table: "JobProfiles",
                column: "CampaignId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobProfiles_OrganizationId",
                table: "JobProfiles",
                column: "OrganizationId");

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
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeImportFailures_OrganizationId",
                table: "ResumeImportFailures",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeImportFailures_ResumeImportId",
                table: "ResumeImportFailures",
                column: "ResumeImportId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeImports_CampaignId",
                table: "ResumeImports",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeImports_OrganizationId",
                table: "ResumeImports",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeProfiles_ApplicationId",
                table: "ResumeProfiles",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeProfiles_OrganizationId",
                table: "ResumeProfiles",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeProfiles_OrganizationId_ApplicationId",
                table: "ResumeProfiles",
                columns: new[] { "OrganizationId", "ApplicationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_ApplicationId",
                table: "Resumes",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_OrganizationId",
                table: "Resumes",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_OrganizationId_ApplicationId",
                table: "Resumes",
                columns: new[] { "OrganizationId", "ApplicationId" });

            migrationBuilder.CreateIndex(
                name: "IX_ResumeScreenings_ApplicationId",
                table: "ResumeScreenings",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeScreenings_OrganizationId",
                table: "ResumeScreenings",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeScreenings_OrganizationId_ApplicationId",
                table: "ResumeScreenings",
                columns: new[] { "OrganizationId", "ApplicationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_OrganizationId",
                table: "Users",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OrganizationId_Email",
                table: "Users",
                columns: new[] { "OrganizationId", "Email" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HiringStrategies");

            migrationBuilder.DropTable(
                name: "JobProfiles");

            migrationBuilder.DropTable(
                name: "ResumeImportFailures");

            migrationBuilder.DropTable(
                name: "ResumeProfiles");

            migrationBuilder.DropTable(
                name: "Resumes");

            migrationBuilder.DropTable(
                name: "ResumeScreenings");

            migrationBuilder.DropTable(
                name: "ResumeImports");

            migrationBuilder.DropTable(
                name: "CandidateApplications");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
