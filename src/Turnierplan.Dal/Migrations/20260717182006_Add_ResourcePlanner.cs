using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Add_ResourcePlanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourcePlanners",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourcePlanners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourcePlanners_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "turnierplan",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IAM_ResourcePlanner",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourcePlannerId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Principal = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IAM_ResourcePlanner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IAM_ResourcePlanner_ResourcePlanners_ResourcePlannerId",
                        column: x => x.ResourcePlannerId,
                        principalSchema: "turnierplan",
                        principalTable: "ResourcePlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceGroups",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResourcePlannerId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceGroups_ResourcePlanners_ResourcePlannerId",
                        column: x => x.ResourcePlannerId,
                        principalSchema: "turnierplan",
                        principalTable: "ResourcePlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourcePlannerViews",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<long>(type: "bigint", nullable: false),
                    ResourcePlannerId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayAllGroups = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourcePlannerViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourcePlannerViews_ResourcePlanners_ResourcePlannerId",
                        column: x => x.ResourcePlannerId,
                        principalSchema: "turnierplan",
                        principalTable: "ResourcePlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                schema: "turnierplan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResourcePlannerId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resources_ResourcePlanners_ResourcePlannerId",
                        column: x => x.ResourcePlannerId,
                        principalSchema: "turnierplan",
                        principalTable: "ResourcePlanners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceGroupResourcePlannerView",
                schema: "turnierplan",
                columns: table => new
                {
                    ResourceGroupsId = table.Column<long>(type: "bigint", nullable: false),
                    ResourcePlannerViewId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceGroupResourcePlannerView", x => new { x.ResourceGroupsId, x.ResourcePlannerViewId });
                    table.ForeignKey(
                        name: "FK_ResourceGroupResourcePlannerView_ResourceGroups_ResourceGro~",
                        column: x => x.ResourceGroupsId,
                        principalSchema: "turnierplan",
                        principalTable: "ResourceGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceGroupResourcePlannerView_ResourcePlannerViews_Resou~",
                        column: x => x.ResourcePlannerViewId,
                        principalSchema: "turnierplan",
                        principalTable: "ResourcePlannerViews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceAssignments",
                schema: "turnierplan",
                columns: table => new
                {
                    ResourceGroupId = table.Column<long>(type: "bigint", nullable: false),
                    ResourceId = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceAssignments", x => new { x.ResourceGroupId, x.ResourceId });
                    table.ForeignKey(
                        name: "FK_ResourceAssignments_ResourceGroups_ResourceGroupId",
                        column: x => x.ResourceGroupId,
                        principalSchema: "turnierplan",
                        principalTable: "ResourceGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceAssignments_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "turnierplan",
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IAM_ResourcePlanner_ResourcePlannerId",
                schema: "turnierplan",
                table: "IAM_ResourcePlanner",
                column: "ResourcePlannerId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAssignments_ResourceId",
                schema: "turnierplan",
                table: "ResourceAssignments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceGroupResourcePlannerView_ResourcePlannerViewId",
                schema: "turnierplan",
                table: "ResourceGroupResourcePlannerView",
                column: "ResourcePlannerViewId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceGroups_ResourcePlannerId",
                schema: "turnierplan",
                table: "ResourceGroups",
                column: "ResourcePlannerId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePlanners_OrganizationId",
                schema: "turnierplan",
                table: "ResourcePlanners",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePlanners_PublicId",
                schema: "turnierplan",
                table: "ResourcePlanners",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePlannerViews_ResourcePlannerId",
                schema: "turnierplan",
                table: "ResourcePlannerViews",
                column: "ResourcePlannerId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ResourcePlannerId",
                schema: "turnierplan",
                table: "Resources",
                column: "ResourcePlannerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IAM_ResourcePlanner",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "ResourceAssignments",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "ResourceGroupResourcePlannerView",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "Resources",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "ResourceGroups",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "ResourcePlannerViews",
                schema: "turnierplan");

            migrationBuilder.DropTable(
                name: "ResourcePlanners",
                schema: "turnierplan");
        }
    }
}
