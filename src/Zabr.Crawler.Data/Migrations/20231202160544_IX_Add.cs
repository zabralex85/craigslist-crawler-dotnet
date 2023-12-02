using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zabr.Craiglists.Crawler.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexAndChangeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Pages",
                type: "NVARCHAR(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.CreateIndex(
                name: "IX_URL",
                table: "Pages",
                column: "Url",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_URL",
                table: "Pages");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Pages",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(250)",
                oldMaxLength: 250);
        }
    }
}
