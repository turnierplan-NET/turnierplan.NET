using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "turnierplan");

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EMail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedEMail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    LastPasswordChange = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SecurityStamp = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "turnierplan",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "turnierplan",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.RoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "turnierplan",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "turnierplan",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    SecretHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "turnierplan",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folders_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "turnierplan",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResourceIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    FileType = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "turnierplan",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AddressDetails = table.Column<List<string>>(type: "text[]", nullable: false),
                    ExternalLinks = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Venues_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "turnierplan",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeyRequests",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApiKeyId = table.Column<long>(type: "bigint", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Path = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeyRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeyRequests_ApiKeys_ApiKeyId",
                        column: x => x.ApiKeyId,
                        principalSchema: "turnierplan",
                        principalTable: "ApiKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    IsMigrated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    FolderId = table.Column<long>(type: "bigint", nullable: true),
                    VenueId = table.Column<long>(type: "bigint", nullable: true),
                    Visibility = table.Column<int>(type: "integer", nullable: false),
                    PublicPageViews = table.Column<int>(type: "integer", nullable: false),
                    OrganizerLogoId = table.Column<long>(type: "bigint", nullable: true),
                    SponsorLogoId = table.Column<long>(type: "bigint", nullable: true),
                    SponsorBannerId = table.Column<long>(type: "bigint", nullable: true),
                    ComputationConfiguration = table.Column<string>(type: "jsonb", nullable: false),
                    MatchPlanConfiguration = table.Column<string>(type: "jsonb", nullable: true),
                    PresentationConfiguration = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tournaments_Folders_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "turnierplan",
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tournaments_Images_OrganizerLogoId",
                        column: x => x.OrganizerLogoId,
                        principalSchema: "turnierplan",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tournaments_Images_SponsorBannerId",
                        column: x => x.SponsorBannerId,
                        principalSchema: "turnierplan",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tournaments_Images_SponsorLogoId",
                        column: x => x.SponsorLogoId,
                        principalSchema: "turnierplan",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tournaments_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "turnierplan",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tournaments_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "turnierplan",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<long>(type: "bigint", nullable: false),
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Configuration = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    GenerationCount = table.Column<int>(type: "integer", nullable: false),
                    LastGeneration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalSchema: "turnierplan",
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    AlphabeticalId = table.Column<char>(type: "character(1)", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => new { x.TournamentId, x.Id });
                    table.ForeignKey(
                        name: "FK_Groups_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalSchema: "turnierplan",
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    OutOfCompetition = table.Column<bool>(type: "boolean", nullable: false),
                    EntryFeePaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => new { x.TournamentId, x.Id });
                    table.ForeignKey(
                        name: "FK_Teams_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalSchema: "turnierplan",
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Court = table.Column<short>(type: "smallint", nullable: false),
                    Kickoff = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TeamSelectorA = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    TeamSelectorB = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: true),
                    FinalsRound = table.Column<int>(type: "integer", nullable: true),
                    PlayoffPosition = table.Column<int>(type: "integer", nullable: true),
                    IsCurrentlyPlaying = table.Column<bool>(type: "boolean", nullable: true),
                    ScoreA = table.Column<int>(type: "integer", nullable: true),
                    ScoreB = table.Column<int>(type: "integer", nullable: true),
                    OutcomeType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => new { x.TournamentId, x.Id });
                    table.ForeignKey(
                        name: "FK_Matches_Groups_TournamentId_GroupId",
                        columns: x => new { x.TournamentId, x.GroupId },
                        principalSchema: "turnierplan",
                        principalTable: "Groups",
                        principalColumns: new[] { "TournamentId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalSchema: "turnierplan",
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupParticipants",
                schema: "turnierplan",
                columns: table => new
                {
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupParticipants", x => new { x.TournamentId, x.GroupId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_GroupParticipants_Groups_TournamentId_GroupId",
                        columns: x => new { x.TournamentId, x.GroupId },
                        principalSchema: "turnierplan",
                        principalTable: "Groups",
                        principalColumns: new[] { "TournamentId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupParticipants_Teams_TournamentId_TeamId",
                        columns: x => new { x.TournamentId, x.TeamId },
                        principalSchema: "turnierplan",
                        principalTable: "Teams",
                        principalColumns: new[] { "TournamentId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "turnierplan",
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("9da7acec-ed66-4698-a2d6-927c9ee3f83a"), "Administrator" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeyRequests_ApiKeyId",
                schema: "turnierplan",
                table: "ApiKeyRequests",
                column: "ApiKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeyRequests_Timestamp",
                schema: "turnierplan",
                table: "ApiKeyRequests",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_OrganizationId",
                schema: "turnierplan",
                table: "ApiKeys",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_PublicId",
                schema: "turnierplan",
                table: "ApiKeys",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PublicId",
                schema: "turnierplan",
                table: "Documents",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_TournamentId",
                schema: "turnierplan",
                table: "Documents",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_OrganizationId",
                schema: "turnierplan",
                table: "Folders",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_PublicId",
                schema: "turnierplan",
                table: "Folders",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupParticipants_TournamentId_TeamId",
                schema: "turnierplan",
                table: "GroupParticipants",
                columns: new[] { "TournamentId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_Images_OrganizationId",
                schema: "turnierplan",
                table: "Images",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_PublicId",
                schema: "turnierplan",
                table: "Images",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_ResourceIdentifier",
                schema: "turnierplan",
                table: "Images",
                column: "ResourceIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TournamentId_GroupId",
                schema: "turnierplan",
                table: "Matches",
                columns: new[] { "TournamentId", "GroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OwnerId",
                schema: "turnierplan",
                table: "Organizations",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_PublicId",
                schema: "turnierplan",
                table: "Organizations",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "turnierplan",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_FolderId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_OrganizationId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_OrganizerLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "OrganizerLogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_PublicId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_SponsorBannerId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "SponsorBannerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_SponsorLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "SponsorLogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_VenueId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                schema: "turnierplan",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEMail",
                schema: "turnierplan",
                table: "Users",
                column: "NormalizedEMail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Venues_OrganizationId",
                schema: "turnierplan",
                table: "Venues",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Venues_PublicId",
                schema: "turnierplan",
                table: "Venues",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeyRequests",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Documents",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "GroupParticipants",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Matches",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "ApiKeys",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Teams",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Tournaments",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Folders",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Images",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Venues",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Organizations",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "turnierplan");
        }
    }
}
