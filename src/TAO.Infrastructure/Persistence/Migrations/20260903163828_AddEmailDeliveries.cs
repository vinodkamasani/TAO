using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAO.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailDeliveries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    table.PrimaryKey(
                        "PK_EmailDeliveries",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_EmailDeliveries_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_EmailDeliveries_CandidateApplications_CandidateApplicationId",
                        column: x => x.CandidateApplicationId,
                        principalTable: "CandidateApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_EmailDeliveries_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                column: "RecipientEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailDeliveries");
        }
    }
}
