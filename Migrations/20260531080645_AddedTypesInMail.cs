using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_paositra.Migrations
{
    /// <inheritdoc />
    public partial class AddedTypesInMail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "MailStatuses");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Mails",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Mails");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "MailStatuses",
                type: "text",
                nullable: true);
        }
    }
}
