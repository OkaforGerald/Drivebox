using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dropbox.Migrations
{
    /// <inheritdoc />
    public partial class SchemaUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileExt",
                table: "Contents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExt",
                table: "Contents");
        }
    }
}
