using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace e_paositra.Migrations
{
    /// <inheritdoc />
    public partial class Remove_MailTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mails_MailTypes_MailTypeId",
                table: "Mails");

            migrationBuilder.DropTable(
                name: "MailTypes");

            migrationBuilder.DropIndex(
                name: "IX_Mails_MailTypeId",
                table: "Mails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BasePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mails_MailTypeId",
                table: "Mails",
                column: "MailTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mails_MailTypes_MailTypeId",
                table: "Mails",
                column: "MailTypeId",
                principalTable: "MailTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
