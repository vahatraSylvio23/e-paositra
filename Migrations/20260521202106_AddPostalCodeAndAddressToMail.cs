using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_paositra.Migrations
{
    /// <inheritdoc />
    public partial class AddPostalCodeAndAddressToMail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Mails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Mails",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Mails");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Mails");
        }
    }
}
