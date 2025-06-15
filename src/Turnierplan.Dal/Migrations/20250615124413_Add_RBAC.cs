using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Add_RBAC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_Users_OwnerId",
                schema: "turnierplan",
                table: "Organizations");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "turnierplan");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_OwnerId",
                schema: "turnierplan",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                schema: "turnierplan",
                table: "Organizations");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdministrator",
                schema: "turnierplan",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "IAM_ApiKey",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApiKeyId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IAM_ApiKey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IAM_ApiKey_ApiKeys_ApiKeyId",
                        column: x => x.ApiKeyId,
                        principalSchema: "turnierplan",
                        principalTable: "ApiKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IAM_Folder",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FolderId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IAM_Folder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IAM_Folder_Folders_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "turnierplan",
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IAM_Image",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IAM_Image", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IAM_Image_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "turnierplan",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IAM_Organization",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IAM_Organization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IAM_Organization_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "turnierplan",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IAM_Tournament",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IAM_Tournament", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IAM_Tournament_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalSchema: "turnierplan",
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IAM_Venue",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VenueId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IAM_Venue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IAM_Venue_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "turnierplan",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IAM_ApiKey_ApiKeyId",
                schema: "turnierplan",
                table: "IAM_ApiKey",
                column: "ApiKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_IAM_Folder_FolderId",
                schema: "turnierplan",
                table: "IAM_Folder",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_IAM_Image_ImageId",
                schema: "turnierplan",
                table: "IAM_Image",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_IAM_Organization_OrganizationId",
                schema: "turnierplan",
                table: "IAM_Organization",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_IAM_Tournament_TournamentId",
                schema: "turnierplan",
                table: "IAM_Tournament",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_IAM_Venue_VenueId",
                schema: "turnierplan",
                table: "IAM_Venue",
                column: "VenueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IAM_ApiKey",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "IAM_Folder",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "IAM_Image",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "IAM_Organization",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "IAM_Tournament",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "IAM_Venue",
                schema: "turnierplan");

            migrationBuilder.DropColumn(
                name: "IsAdministrator",
                schema: "turnierplan",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                schema: "turnierplan",
                table: "Organizations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.InsertData(
                schema: "turnierplan",
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("9da7acec-ed66-4698-a2d6-927c9ee3f83a"), "Administrator" });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OwnerId",
                schema: "turnierplan",
                table: "Organizations",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "turnierplan",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                schema: "turnierplan",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_Users_OwnerId",
                schema: "turnierplan",
                table: "Organizations",
                column: "OwnerId",
                principalSchema: "turnierplan",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
