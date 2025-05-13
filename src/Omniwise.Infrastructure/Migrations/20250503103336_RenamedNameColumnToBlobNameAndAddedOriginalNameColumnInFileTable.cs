using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omniwise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamedNameColumnToBlobNameAndAddedOriginalNameColumnInFileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Files",
                newName: "OriginalName");

            migrationBuilder.AddColumn<string>(
                name: "BlobName",
                table: "Files",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobName",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "OriginalName",
                table: "Files",
                newName: "Name");
        }
    }
}
