using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPano2.Migrations
{
    /// <inheritdoc />
    public partial class VideoConfigDefaultActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultActive",
                table: "VideoConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefaultActive",
                table: "VideoConfigs");
        }
    }
}
