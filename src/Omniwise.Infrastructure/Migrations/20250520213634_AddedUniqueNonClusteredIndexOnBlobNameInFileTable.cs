using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omniwise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedUniqueNonClusteredIndexOnBlobNameInFileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Files_BlobName",
                table: "Files",
                column: "BlobName",
                unique: true)
                .Annotation("SqlServer:Clustered", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_BlobName",
                table: "Files");
        }
    }
}
