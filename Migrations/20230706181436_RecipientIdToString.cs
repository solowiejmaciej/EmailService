using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackgroundEmailServiceTest.Migrations
{
    /// <inheritdoc />
    public partial class RecipientIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailsNotifications_Recipient_RecipientId",
                table: "EmailsNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_PushNotifications_Recipient_RecipientId",
                table: "PushNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsNotifications_Recipient_RecipientId",
                table: "SmsNotifications");

            migrationBuilder.DropTable(
                name: "Recipient");

            migrationBuilder.DropIndex(
                name: "IX_SmsNotifications_RecipientId",
                table: "SmsNotifications");

            migrationBuilder.DropIndex(
                name: "IX_PushNotifications_RecipientId",
                table: "PushNotifications");

            migrationBuilder.DropIndex(
                name: "IX_EmailsNotifications_RecipientId",
                table: "EmailsNotifications");

            migrationBuilder.AlterColumn<string>(
                name: "RecipientId",
                table: "SmsNotifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "RecipientId",
                table: "PushNotifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "RecipientId",
                table: "EmailsNotifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RecipientId",
                table: "SmsNotifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "RecipientId",
                table: "PushNotifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "RecipientId",
                table: "EmailsNotifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Recipient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipient", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmsNotifications_RecipientId",
                table: "SmsNotifications",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_PushNotifications_RecipientId",
                table: "PushNotifications",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailsNotifications_RecipientId",
                table: "EmailsNotifications",
                column: "RecipientId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailsNotifications_Recipient_RecipientId",
                table: "EmailsNotifications",
                column: "RecipientId",
                principalTable: "Recipient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PushNotifications_Recipient_RecipientId",
                table: "PushNotifications",
                column: "RecipientId",
                principalTable: "Recipient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsNotifications_Recipient_RecipientId",
                table: "SmsNotifications",
                column: "RecipientId",
                principalTable: "Recipient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}