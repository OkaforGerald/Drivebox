using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dropbox.Migrations
{
    /// <inheritdoc />
    public partial class Backup4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PathOnLocal",
                table: "Folders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PathOnLocal",
                table: "Folders");
        }
    }
}
