using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_paositra.Migrations
{
    /// <inheritdoc />
    public partial class Changed_Mail1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "Mails",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "Mails");
        }
    }
}
