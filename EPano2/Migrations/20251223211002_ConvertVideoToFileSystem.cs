using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPano2.Migrations
{
    /// <inheritdoc />
    public partial class ConvertVideoToFileSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefaultActive",
                table: "VideoConfigs");

            migrationBuilder.RenameColumn(
                name: "DefaultVideoUrl",
                table: "VideoConfigs",
                newName: "DefaultVideoFilePath");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "ExtraVideoLinks",
                newName: "FilePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DefaultVideoFilePath",
                table: "VideoConfigs",
                newName: "DefaultVideoUrl");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "ExtraVideoLinks",
                newName: "Url");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultActive",
                table: "VideoConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
