using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_paositra.Migrations
{
    /// <inheritdoc />
    public partial class AddRoutingToMail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "Mails",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Mails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EndAgency",
                table: "Mails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartAgency",
                table: "Mails",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Mails");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Mails");

            migrationBuilder.DropColumn(
                name: "EndAgency",
                table: "Mails");

            migrationBuilder.DropColumn(
                name: "StartAgency",
                table: "Mails");
        }
    }
}
