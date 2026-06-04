using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Rename_TournamentPlannerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_PlanningRealms_TournamentPlannerId",
                schema: "turnierplan",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_IAM_PlanningRealm_PlanningRealms_TournamentPlannerId",
                schema: "turnierplan",
                table: "IAM_PlanningRealm");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationLinks_PlanningRealms_TournamentPlannerId",
                schema: "turnierplan",
                table: "InvitationLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_PlanningRealms_TournamentPlannerId",
                schema: "turnierplan",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanningRealms_Organizations_OrganizationId",
                schema: "turnierplan",
                table: "PlanningRealms");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentClasses_PlanningRealms_TournamentPlannerId",
                schema: "turnierplan",
                table: "TournamentClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlanningRealms",
                schema: "turnierplan",
                table: "PlanningRealms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IAM_PlanningRealm",
                schema: "turnierplan",
                table: "IAM_PlanningRealm");

            migrationBuilder.RenameTable(
                name: "PlanningRealms",
                schema: "turnierplan",
                newName: "TournamentPlanners",
                newSchema: "turnierplan");

            migrationBuilder.RenameTable(
                name: "IAM_PlanningRealm",
                schema: "turnierplan",
                newName: "IAM_TournamentPlanner",
                newSchema: "turnierplan");

            migrationBuilder.RenameIndex(
                name: "IX_PlanningRealms_PublicId",
                schema: "turnierplan",
                table: "TournamentPlanners",
                newName: "IX_TournamentPlanners_PublicId");

            migrationBuilder.RenameIndex(
                name: "IX_PlanningRealms_OrganizationId",
                schema: "turnierplan",
                table: "TournamentPlanners",
                newName: "IX_TournamentPlanners_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_IAM_PlanningRealm_TournamentPlannerId",
                schema: "turnierplan",
                table: "IAM_TournamentPlanner",
                newName: "IX_IAM_TournamentPlanner_TournamentPlannerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentPlanners",
                schema: "turnierplan",
                table: "TournamentPlanners",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IAM_TournamentPlanner",
                schema: "turnierplan",
                table: "IAM_TournamentPlanner",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_TournamentPlanners_TournamentPlannerId",
                schema: "turnierplan",
                table: "Applications",
                column: "TournamentPlannerId",
                principalSchema: "turnierplan",
                principalTable: "TournamentPlanners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IAM_TournamentPlanner_TournamentPlanners_TournamentPlannerId",
                schema: "turnierplan",
                table: "IAM_TournamentPlanner",
                column: "TournamentPlannerId",
                principalSchema: "turnierplan",
                principalTable: "TournamentPlanners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationLinks_TournamentPlanners_TournamentPlannerId",
                schema: "turnierplan",
                table: "InvitationLinks",
                column: "TournamentPlannerId",
                principalSchema: "turnierplan",
                principalTable: "TournamentPlanners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_TournamentPlanners_TournamentPlannerId",
                schema: "turnierplan",
                table: "Labels",
                column: "TournamentPlannerId",
                principalSchema: "turnierplan",
                principalTable: "TournamentPlanners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentClasses_TournamentPlanners_TournamentPlannerId",
                schema: "turnierplan",
                table: "TournamentClasses",
                column: "TournamentPlannerId",
                principalSchema: "turnierplan",
                principalTable: "TournamentPlanners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentPlanners_Organizations_OrganizationId",
                schema: "turnierplan",
                table: "TournamentPlanners",
                column: "OrganizationId",
                principalSchema: "turnierplan",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_TournamentPlanners_TournamentPlannerId",
                schema: "turnierplan",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_IAM_TournamentPlanner_TournamentPlanners_TournamentPlannerId",
                schema: "turnierplan",
                table: "IAM_TournamentPlanner");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationLinks_TournamentPlanners_TournamentPlannerId",
                schema: "turnierplan",
                table: "InvitationLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_TournamentPlanners_TournamentPlannerId",
                schema: "turnierplan",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentClasses_TournamentPlanners_TournamentPlannerId",
                schema: "turnierplan",
                table: "TournamentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentPlanners_Organizations_OrganizationId",
                schema: "turnierplan",
                table: "TournamentPlanners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentPlanners",
                schema: "turnierplan",
                table: "TournamentPlanners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IAM_TournamentPlanner",
                schema: "turnierplan",
                table: "IAM_TournamentPlanner");

            migrationBuilder.RenameTable(
                name: "TournamentPlanners",
                schema: "turnierplan",
                newName: "PlanningRealms",
                newSchema: "turnierplan");

            migrationBuilder.RenameTable(
                name: "IAM_TournamentPlanner",
                schema: "turnierplan",
                newName: "IAM_PlanningRealm",
                newSchema: "turnierplan");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentPlanners_PublicId",
                schema: "turnierplan",
                table: "PlanningRealms",
                newName: "IX_PlanningRealms_PublicId");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentPlanners_OrganizationId",
                schema: "turnierplan",
                table: "PlanningRealms",
                newName: "IX_PlanningRealms_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_IAM_TournamentPlanner_TournamentPlannerId",
                schema: "turnierplan",
                table: "IAM_PlanningRealm",
                newName: "IX_IAM_PlanningRealm_TournamentPlannerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlanningRealms",
                schema: "turnierplan",
                table: "PlanningRealms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IAM_PlanningRealm",
                schema: "turnierplan",
                table: "IAM_PlanningRealm",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_PlanningRealms_TournamentPlannerId",
                schema: "turnierplan",
                table: "Applications",
                column: "TournamentPlannerId",
                principalSchema: "turnierplan",
                principalTable: "PlanningRealms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IAM_PlanningRealm_PlanningRealms_TournamentPlannerId",
                schema: "turnierplan",
                table: "IAM_PlanningRealm",
                column: "TournamentPlannerId",
                principalSchema: "turnierplan",
                principalTable: "PlanningRealms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationLinks_PlanningRealms_TournamentPlannerId",
                schema: "turnierplan",
                table: "InvitationLinks",
                column: "TournamentPlannerId",
                principalSchema: "turnierplan",
                principalTable: "PlanningRealms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_PlanningRealms_TournamentPlannerId",
                schema: "turnierplan",
                table: "Labels",
                column: "TournamentPlannerId",
                principalSchema: "turnierplan",
                principalTable: "PlanningRealms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanningRealms_Organizations_OrganizationId",
                schema: "turnierplan",
                table: "PlanningRealms",
                column: "OrganizationId",
                principalSchema: "turnierplan",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentClasses_PlanningRealms_TournamentPlannerId",
                schema: "turnierplan",
                table: "TournamentClasses",
                column: "TournamentPlannerId",
                principalSchema: "turnierplan",
                principalTable: "PlanningRealms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
