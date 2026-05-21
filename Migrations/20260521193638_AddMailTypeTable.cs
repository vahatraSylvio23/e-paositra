using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace e_paositra.Migrations
{
    /// <inheritdoc />
    public partial class AddMailTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "sender",
                table: "Mails",
                newName: "Sender");

            migrationBuilder.RenameColumn(
                name: "reference",
                table: "Mails",
                newName: "Reference");

            migrationBuilder.RenameColumn(
                name: "recipient",
                table: "Mails",
                newName: "Recipient");

            migrationBuilder.RenameColumn(
                name: "MailtypeId",
                table: "Mails",
                newName: "MailTypeId");

            migrationBuilder.CreateTable(
                name: "MailTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mails_MailTypeId",
                table: "Mails",
                column: "MailTypeId");

            // Seed default MailType data
            migrationBuilder.InsertData(
                table: "MailTypes",
                columns: new[] { "Name", "BasePrice" },
                values: new object[,]
                {
                    { "Lettre", 5.00m },
                    { "Colis", 15.00m },
                    { "Recommandé", 20.00m },
                    { "Express", 30.00m },
                    { "Standard", 10.00m }
                });

            // Update existing Mail records with default MailTypeId if null or invalid
            migrationBuilder.Sql("UPDATE \"Mails\" SET \"MailTypeId\" = 1 WHERE \"MailTypeId\" IS NULL OR \"MailTypeId\" NOT IN (SELECT \"Id\" FROM \"MailTypes\")");

            migrationBuilder.AddForeignKey(
                name: "FK_Mails_MailTypes_MailTypeId",
                table: "Mails",
                column: "MailTypeId",
                principalTable: "MailTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mails_MailTypes_MailTypeId",
                table: "Mails");

            migrationBuilder.DropTable(
                name: "MailTypes");

            migrationBuilder.DropIndex(
                name: "IX_Mails_MailTypeId",
                table: "Mails");

            migrationBuilder.RenameColumn(
                name: "Sender",
                table: "Mails",
                newName: "sender");

            migrationBuilder.RenameColumn(
                name: "Reference",
                table: "Mails",
                newName: "reference");

            migrationBuilder.RenameColumn(
                name: "Recipient",
                table: "Mails",
                newName: "recipient");

            migrationBuilder.RenameColumn(
                name: "MailTypeId",
                table: "Mails",
                newName: "MailtypeId");
        }
    }
}
