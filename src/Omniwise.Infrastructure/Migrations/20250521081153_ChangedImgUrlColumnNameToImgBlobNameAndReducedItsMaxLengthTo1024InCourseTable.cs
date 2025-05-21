using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omniwise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedImgUrlColumnNameToImgBlobNameAndReducedItsMaxLengthTo1024InCourseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "ImgBlobName",
                table: "Courses",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgBlobName",
                table: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "Courses",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }
    }
}
