using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackgroundEmailServiceTest.Migrations
{
    /// <inheritdoc />
    public partial class UserIdToPushFieldAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeviceToken",
                table: "PushNotifications",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "PushNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "PushNotifications");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PushNotifications",
                newName: "DeviceToken");
        }
    }
}
