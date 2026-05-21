using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_paositra.Migrations
{
    /// <inheritdoc />
    public partial class AddMailIdToMailStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MailId",
                table: "MailStatuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MailStatuses_MailId",
                table: "MailStatuses",
                column: "MailId");

            migrationBuilder.AddForeignKey(
                name: "FK_MailStatuses_Mails_MailId",
                table: "MailStatuses",
                column: "MailId",
                principalTable: "Mails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailStatuses_Mails_MailId",
                table: "MailStatuses");

            migrationBuilder.DropIndex(
                name: "IX_MailStatuses_MailId",
                table: "MailStatuses");

            migrationBuilder.DropColumn(
                name: "MailId",
                table: "MailStatuses");
        }
    }
}
