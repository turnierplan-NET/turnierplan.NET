using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Add_PlanningRealm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanningRealms",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningRealms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanningRealms_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "turnierplan",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IAM_PlanningRealm",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanningRealmId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IAM_PlanningRealm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IAM_PlanningRealm_PlanningRealms_PlanningRealmId",
                        column: x => x.PlanningRealmId,
                        principalSchema: "turnierplan",
                        principalTable: "PlanningRealms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvitationLinks",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    ColorCode = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContactTelephone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PrimaryLogoId = table.Column<long>(type: "bigint", nullable: true),
                    SecondaryLogoId = table.Column<long>(type: "bigint", nullable: true),
                    PlanningRealmId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalLinks = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvitationLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvitationLinks_Images_PrimaryLogoId",
                        column: x => x.PrimaryLogoId,
                        principalSchema: "turnierplan",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InvitationLinks_Images_SecondaryLogoId",
                        column: x => x.SecondaryLogoId,
                        principalSchema: "turnierplan",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InvitationLinks_PlanningRealms_PlanningRealmId",
                        column: x => x.PlanningRealmId,
                        principalSchema: "turnierplan",
                        principalTable: "PlanningRealms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentClasses",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    MaxTeamCount = table.Column<int>(type: "integer", nullable: true),
                    PlanningRealmId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentClasses_PlanningRealms_PlanningRealmId",
                        column: x => x.PlanningRealmId,
                        principalSchema: "turnierplan",
                        principalTable: "PlanningRealms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceLinkId = table.Column<long>(type: "bigint", nullable: true),
                    Tag = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Telephone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CommentFromSender = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_InvitationLinks_SourceLinkId",
                        column: x => x.SourceLinkId,
                        principalSchema: "turnierplan",
                        principalTable: "InvitationLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InvitationLinkEntries",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClassId = table.Column<long>(type: "bigint", nullable: false),
                    MaxTeamsPerRegistration = table.Column<int>(type: "integer", nullable: true),
                    AllowNewRegistrations = table.Column<bool>(type: "boolean", nullable: false),
                    InvitationLinkId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvitationLinkEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvitationLinkEntries_InvitationLinks_InvitationLinkId",
                        column: x => x.InvitationLinkId,
                        principalSchema: "turnierplan",
                        principalTable: "InvitationLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvitationLinkEntries_TournamentClasses_ClassId",
                        column: x => x.ClassId,
                        principalSchema: "turnierplan",
                        principalTable: "TournamentClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationTeams",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    ClassId = table.Column<long>(type: "bigint", nullable: false),
                    ApplicationId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationTeams_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "turnierplan",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationTeams_TournamentClasses_ClassId",
                        column: x => x.ClassId,
                        principalSchema: "turnierplan",
                        principalTable: "TournamentClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_SourceLinkId",
                schema: "turnierplan",
                table: "Applications",
                column: "SourceLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationTeams_ApplicationId",
                schema: "turnierplan",
                table: "ApplicationTeams",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationTeams_ClassId",
                schema: "turnierplan",
                table: "ApplicationTeams",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_IAM_PlanningRealm_PlanningRealmId",
                schema: "turnierplan",
                table: "IAM_PlanningRealm",
                column: "PlanningRealmId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationLinkEntries_ClassId",
                schema: "turnierplan",
                table: "InvitationLinkEntries",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationLinkEntries_InvitationLinkId",
                schema: "turnierplan",
                table: "InvitationLinkEntries",
                column: "InvitationLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationLinks_PlanningRealmId",
                schema: "turnierplan",
                table: "InvitationLinks",
                column: "PlanningRealmId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationLinks_PrimaryLogoId",
                schema: "turnierplan",
                table: "InvitationLinks",
                column: "PrimaryLogoId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationLinks_PublicId",
                schema: "turnierplan",
                table: "InvitationLinks",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvitationLinks_SecondaryLogoId",
                schema: "turnierplan",
                table: "InvitationLinks",
                column: "SecondaryLogoId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningRealms_OrganizationId",
                schema: "turnierplan",
                table: "PlanningRealms",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningRealms_PublicId",
                schema: "turnierplan",
                table: "PlanningRealms",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentClasses_PlanningRealmId",
                schema: "turnierplan",
                table: "TournamentClasses",
                column: "PlanningRealmId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationTeams",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "IAM_PlanningRealm",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "InvitationLinkEntries",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Applications",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "TournamentClasses",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "InvitationLinks",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "PlanningRealms",
                schema: "turnierplan");
        }
    }
}
