using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackgroundEmailServiceTest.Migrations
{
    /// <inheritdoc />
    public partial class SmtpAccountFieldRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SmtpAccount",
                table: "Emails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SmtpAccount",
                table: "Emails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}