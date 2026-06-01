using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_paositra.Migrations
{
    /// <inheritdoc />
    public partial class Added_type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Mails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Mails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
