using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dropbox.Migrations
{
    /// <inheritdoc />
    public partial class ForBackup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnLocal",
                table: "Folders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnLocal",
                table: "Folders");
        }
    }
}
