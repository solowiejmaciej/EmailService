using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackgroundEmailServiceTest.Migrations
{
    /// <inheritdoc />
    public partial class RecipientsRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailsNotifications_Recipients_RecipientId",
                table: "EmailsNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_PushNotifications_Recipients_RecipientId",
                table: "PushNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsNotifications_Recipients_RecipientId",
                table: "SmsNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipients",
                table: "Recipients");

            migrationBuilder.RenameTable(
                name: "Recipients",
                newName: "Recipient");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipient",
                table: "Recipient",
                column: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipient",
                table: "Recipient");

            migrationBuilder.RenameTable(
                name: "Recipient",
                newName: "Recipients");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipients",
                table: "Recipients",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailsNotifications_Recipients_RecipientId",
                table: "EmailsNotifications",
                column: "RecipientId",
                principalTable: "Recipients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PushNotifications_Recipients_RecipientId",
                table: "PushNotifications",
                column: "RecipientId",
                principalTable: "Recipients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsNotifications_Recipients_RecipientId",
                table: "SmsNotifications",
                column: "RecipientId",
                principalTable: "Recipients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}