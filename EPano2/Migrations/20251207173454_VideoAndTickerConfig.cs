using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPano2.Migrations
{
    /// <inheritdoc />
    public partial class VideoAndTickerConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TickerConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TickerConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DefaultVideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtraVideoLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoConfigId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraVideoLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraVideoLinks_VideoConfigs_VideoConfigId",
                        column: x => x.VideoConfigId,
                        principalTable: "VideoConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExtraVideoLinks_VideoConfigId",
                table: "ExtraVideoLinks",
                column: "VideoConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExtraVideoLinks");

            migrationBuilder.DropTable(
                name: "TickerConfigs");

            migrationBuilder.DropTable(
                name: "VideoConfigs");
        }
    }
}
