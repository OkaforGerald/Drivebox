using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dropbox.Migrations
{
    /// <inheritdoc />
    public partial class ForBackup3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Backups",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "Backups");
        }
    }
}
