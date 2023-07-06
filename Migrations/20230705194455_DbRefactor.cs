using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackgroundEmailServiceTest.Migrations
{
    /// <inheritdoc />
    public partial class DbRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "SmsNotifications");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "SmsNotifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PushNotifications");

            migrationBuilder.RenameColumn(
                name: "To",
                table: "SmsNotifications",
                newName: "Content");

            migrationBuilder.AddColumn<int>(
                name: "RecipientId",
                table: "SmsNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecipientId",
                table: "PushNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Recipients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailsNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecipientId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailsNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailsNotifications_Recipients_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Recipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PushNotifications_Recipients_RecipientId",
                table: "PushNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsNotifications_Recipients_RecipientId",
                table: "SmsNotifications");

            migrationBuilder.DropTable(
                name: "EmailsNotifications");

            migrationBuilder.DropTable(
                name: "Recipients");

            migrationBuilder.DropIndex(
                name: "IX_SmsNotifications_RecipientId",
                table: "SmsNotifications");

            migrationBuilder.DropIndex(
                name: "IX_PushNotifications_RecipientId",
                table: "PushNotifications");

            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "SmsNotifications");

            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "PushNotifications");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "SmsNotifications",
                newName: "To");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "SmsNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "SmsNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PushNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailFrom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailSenderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailStatus = table.Column<int>(type: "int", nullable: false),
                    EmailTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.Id);
                });
        }
    }
}