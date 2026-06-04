using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Rename_TournamentPlanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_PlanningRealms_PlanningRealmId",
                schema: "turnierplan",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_IAM_PlanningRealm_PlanningRealms_PlanningRealmId",
                schema: "turnierplan",
                table: "IAM_PlanningRealm");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationLinks_PlanningRealms_PlanningRealmId",
                schema: "turnierplan",
                table: "InvitationLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_PlanningRealms_PlanningRealmId",
                schema: "turnierplan",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentClasses_PlanningRealms_PlanningRealmId",
                schema: "turnierplan",
                table: "TournamentClasses");

            migrationBuilder.RenameColumn(
                name: "PlanningRealmId",
                schema: "turnierplan",
                table: "TournamentClasses",
                newName: "TournamentPlannerId");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentClasses_PlanningRealmId",
                schema: "turnierplan",
                table: "TournamentClasses",
                newName: "IX_TournamentClasses_TournamentPlannerId");

            migrationBuilder.RenameColumn(
                name: "PlanningRealmId",
                schema: "turnierplan",
                table: "Labels",
                newName: "TournamentPlannerId");

            migrationBuilder.RenameIndex(
                name: "IX_Labels_PlanningRealmId",
                schema: "turnierplan",
                table: "Labels",
                newName: "IX_Labels_TournamentPlannerId");

            migrationBuilder.RenameColumn(
                name: "PlanningRealmId",
                schema: "turnierplan",
                table: "InvitationLinks",
                newName: "TournamentPlannerId");

            migrationBuilder.RenameIndex(
                name: "IX_InvitationLinks_PlanningRealmId",
                schema: "turnierplan",
                table: "InvitationLinks",
                newName: "IX_InvitationLinks_TournamentPlannerId");

            migrationBuilder.RenameColumn(
                name: "PlanningRealmId",
                schema: "turnierplan",
                table: "IAM_PlanningRealm",
                newName: "TournamentPlannerId");

            migrationBuilder.RenameIndex(
                name: "IX_IAM_PlanningRealm_PlanningRealmId",
                schema: "turnierplan",
                table: "IAM_PlanningRealm",
                newName: "IX_IAM_PlanningRealm_TournamentPlannerId");

            migrationBuilder.RenameColumn(
                name: "PlanningRealmId",
                schema: "turnierplan",
                table: "Applications",
                newName: "TournamentPlannerId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_PlanningRealmId",
                schema: "turnierplan",
                table: "Applications",
                newName: "IX_Applications_TournamentPlannerId");

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
                name: "FK_TournamentClasses_PlanningRealms_TournamentPlannerId",
                schema: "turnierplan",
                table: "TournamentClasses",
                column: "TournamentPlannerId",
                principalSchema: "turnierplan",
                principalTable: "PlanningRealms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "FK_TournamentClasses_PlanningRealms_TournamentPlannerId",
                schema: "turnierplan",
                table: "TournamentClasses");

            migrationBuilder.RenameColumn(
                name: "TournamentPlannerId",
                schema: "turnierplan",
                table: "TournamentClasses",
                newName: "PlanningRealmId");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentClasses_TournamentPlannerId",
                schema: "turnierplan",
                table: "TournamentClasses",
                newName: "IX_TournamentClasses_PlanningRealmId");

            migrationBuilder.RenameColumn(
                name: "TournamentPlannerId",
                schema: "turnierplan",
                table: "Labels",
                newName: "PlanningRealmId");

            migrationBuilder.RenameIndex(
                name: "IX_Labels_TournamentPlannerId",
                schema: "turnierplan",
                table: "Labels",
                newName: "IX_Labels_PlanningRealmId");

            migrationBuilder.RenameColumn(
                name: "TournamentPlannerId",
                schema: "turnierplan",
                table: "InvitationLinks",
                newName: "PlanningRealmId");

            migrationBuilder.RenameIndex(
                name: "IX_InvitationLinks_TournamentPlannerId",
                schema: "turnierplan",
                table: "InvitationLinks",
                newName: "IX_InvitationLinks_PlanningRealmId");

            migrationBuilder.RenameColumn(
                name: "TournamentPlannerId",
                schema: "turnierplan",
                table: "IAM_PlanningRealm",
                newName: "PlanningRealmId");

            migrationBuilder.RenameIndex(
                name: "IX_IAM_PlanningRealm_TournamentPlannerId",
                schema: "turnierplan",
                table: "IAM_PlanningRealm",
                newName: "IX_IAM_PlanningRealm_PlanningRealmId");

            migrationBuilder.RenameColumn(
                name: "TournamentPlannerId",
                schema: "turnierplan",
                table: "Applications",
                newName: "PlanningRealmId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_TournamentPlannerId",
                schema: "turnierplan",
                table: "Applications",
                newName: "IX_Applications_PlanningRealmId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_PlanningRealms_PlanningRealmId",
                schema: "turnierplan",
                table: "Applications",
                column: "PlanningRealmId",
                principalSchema: "turnierplan",
                principalTable: "PlanningRealms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IAM_PlanningRealm_PlanningRealms_PlanningRealmId",
                schema: "turnierplan",
                table: "IAM_PlanningRealm",
                column: "PlanningRealmId",
                principalSchema: "turnierplan",
                principalTable: "PlanningRealms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationLinks_PlanningRealms_PlanningRealmId",
                schema: "turnierplan",
                table: "InvitationLinks",
                column: "PlanningRealmId",
                principalSchema: "turnierplan",
                principalTable: "PlanningRealms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_PlanningRealms_PlanningRealmId",
                schema: "turnierplan",
                table: "Labels",
                column: "PlanningRealmId",
                principalSchema: "turnierplan",
                principalTable: "PlanningRealms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentClasses_PlanningRealms_PlanningRealmId",
                schema: "turnierplan",
                table: "TournamentClasses",
                column: "PlanningRealmId",
                principalSchema: "turnierplan",
                principalTable: "PlanningRealms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
