using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackgroundEmailServiceTest.Migrations
{
    /// <inheritdoc />
    public partial class StatusFieldAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasErrors",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "isEmailSended",
                table: "Emails");

            migrationBuilder.AddColumn<int>(
                name: "EmailStatus",
                table: "Emails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailStatus",
                table: "Emails");

            migrationBuilder.AddColumn<bool>(
                name: "HasErrors",
                table: "Emails",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Emails",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isEmailSended",
                table: "Emails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}