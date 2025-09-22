using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnierplan.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Add_UserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Note: This migration was modified manually!

            // pre-migration:  Name & EMail are both required columns
            // post-migration: Only UserName is required column, FullName and EMail are optional

            // migration steps:
            //   1. Create new required "UserName" column with a placeholder default value
            //   2. Update all users and set the "UserName" to the current value of the "EMail" column
            //   3. Rename the previous "Name" column to "FullName" and make that column optional
            //   4. Make the "EMail" and "NormalizedEMail" columns optional
            //   5. Create the unique index on the "UserName" column

            // step 1:
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "turnierplan",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "x");

            // step 2:
            migrationBuilder.Sql("""
UPDATE turnierplan."Users" SET "UserName" = "EMail";
""");

            // step 3:
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "turnierplan",
                table: "Users",
                newName: "FullName");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                schema: "turnierplan",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            // step 4:
            migrationBuilder.AlterColumn<string>(
                name: "EMail",
                schema: "turnierplan",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEMail",
                schema: "turnierplan",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            // step 5:
            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                schema: "turnierplan",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Note: This migration was modified manually!

            // pre-migration:  Only UserName is required column, FullName and EMail are optional
            // post-migration: Name & EMail are both required columns

            // migration steps:
            //   1. Delete the unique index on the "UserName" column
            //   2. Make the "EMail" and "NormalizedEMail" columns required
            //   3. Rename the previous "FullName" column to "Name" and make that column required
            //   4. Delete the "UserName" column

            // step 1:
            migrationBuilder.DropIndex(
                name: "IX_Users_UserName",
                schema: "turnierplan",
                table: "Users");

            // step 2:
            migrationBuilder.AlterColumn<string>(
                name: "EMail",
                schema: "turnierplan",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEMail",
                schema: "turnierplan",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // step 3:
            migrationBuilder.RenameColumn(
                name: "FullName",
                schema: "turnierplan",
                table: "Users",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "turnierplan",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // step 4:
            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "turnierplan",
                table: "Users");
        }
    }
}
