using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Add_Rbac : Migration
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

            // The call to DropColumn() was generated here.
            // See comment at the end of method

            migrationBuilder.AddColumn<bool>(
                name: "IsAdministrator",
                schema: "turnierplan",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PrincipalId",
                schema: "turnierplan",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PrincipalId",
                schema: "turnierplan",
                table: "ApiKeys",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "IAM_ApiKey",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApiKeyId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FolderId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.Sql("""
UPDATE turnierplan."ApiKeys" SET "PrincipalId" = gen_random_uuid();
UPDATE turnierplan."Users" SET "PrincipalId" = gen_random_uuid();
""");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PrincipalId",
                schema: "turnierplan",
                table: "Users",
                column: "PrincipalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_PrincipalId",
                schema: "turnierplan",
                table: "ApiKeys",
                column: "PrincipalId",
                unique: true);

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

            // The DropColumn() call was moved here manually. This is done so that the
            // corresponding role assignments can be created before the data is destroyed.

            // 1000 is the numerical value for the "Owner" role

            migrationBuilder.Sql("""
INSERT INTO turnierplan."IAM_Organization" ("Id", "OrganizationId", "CreatedAt", "Role", "Principal", "Description")
SELECT gen_random_uuid(), "Organizations"."Id", NOW(), 1000, ('User:' || "Users"."PrincipalId"), ''
FROM turnierplan."Organizations"
INNER JOIN turnierplan."Users" ON "Organizations"."OwnerId" = "Users"."Id";
""");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                schema: "turnierplan",
                table: "Organizations");
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

            migrationBuilder.DropIndex(
                name: "IX_Users_PrincipalId",
                schema: "turnierplan",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_PrincipalId",
                schema: "turnierplan",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "IsAdministrator",
                schema: "turnierplan",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrincipalId",
                schema: "turnierplan",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrincipalId",
                schema: "turnierplan",
                table: "ApiKeys");

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
