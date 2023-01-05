using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alarms.Migrations
{
    /// <inheritdoc />
    public partial class SeparateUserIdandDateTimeOffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Alarms",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Alarms",
                newName: "OwnerId");
        }
    }
}
