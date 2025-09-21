using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Rename_TournamentImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Images_OrganizerLogoId",
                schema: "turnierplan",
                table: "Tournaments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Images_SponsorBannerId",
                schema: "turnierplan",
                table: "Tournaments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Images_SponsorLogoId",
                schema: "turnierplan",
                table: "Tournaments");

            migrationBuilder.RenameColumn(
                name: "SponsorLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                newName: "SecondaryLogoId");

            // Note: The auto-generated migration swapped the PrimaryImageId and BannerImageId
            // columns during the renaming. This was corrected manually in this migration.

            migrationBuilder.RenameColumn(
                name: "SponsorBannerId",
                schema: "turnierplan",
                table: "Tournaments",
                newName: "BannerImageId");

            migrationBuilder.RenameColumn(
                name: "OrganizerLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                newName: "PrimaryLogoId");

            migrationBuilder.RenameIndex(
                name: "IX_Tournaments_SponsorLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                newName: "IX_Tournaments_SecondaryLogoId");

            migrationBuilder.RenameIndex(
                name: "IX_Tournaments_SponsorBannerId",
                schema: "turnierplan",
                table: "Tournaments",
                newName: "IX_Tournaments_BannerImageId");

            migrationBuilder.RenameIndex(
                name: "IX_Tournaments_OrganizerLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                newName: "IX_Tournaments_PrimaryLogoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_Images_BannerImageId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "BannerImageId",
                principalSchema: "turnierplan",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_Images_PrimaryLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "PrimaryLogoId",
                principalSchema: "turnierplan",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_Images_SecondaryLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "SecondaryLogoId",
                principalSchema: "turnierplan",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Images_BannerImageId",
                schema: "turnierplan",
                table: "Tournaments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Images_PrimaryLogoId",
                schema: "turnierplan",
                table: "Tournaments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Images_SecondaryLogoId",
                schema: "turnierplan",
                table: "Tournaments");

            migrationBuilder.RenameColumn(
                name: "SecondaryLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                newName: "SponsorLogoId");

            // Note: The auto-generated migration swapped the PrimaryImageId and BannerImageId
            // columns during the renaming. This was corrected manually in this migration.

            migrationBuilder.RenameColumn(
                name: "PrimaryLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                newName: "OrganizerLogoId");

            migrationBuilder.RenameColumn(
                name: "BannerImageId",
                schema: "turnierplan",
                table: "Tournaments",
                newName: "SponsorBannerId");

            migrationBuilder.RenameIndex(
                name: "IX_Tournaments_SecondaryLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                newName: "IX_Tournaments_SponsorLogoId");

            migrationBuilder.RenameIndex(
                name: "IX_Tournaments_PrimaryLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                newName: "IX_Tournaments_OrganizerLogoId");

            migrationBuilder.RenameIndex(
                name: "IX_Tournaments_BannerImageId",
                schema: "turnierplan",
                table: "Tournaments",
                newName: "IX_Tournaments_SponsorBannerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_Images_OrganizerLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "OrganizerLogoId",
                principalSchema: "turnierplan",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_Images_SponsorBannerId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "SponsorBannerId",
                principalSchema: "turnierplan",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_Images_SponsorLogoId",
                schema: "turnierplan",
                table: "Tournaments",
                column: "SponsorLogoId",
                principalSchema: "turnierplan",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
