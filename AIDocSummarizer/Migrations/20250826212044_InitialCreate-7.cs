using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIDocSummarizer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContextText",
                table: "Documents",
                newName: "ContentText");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContentText",
                table: "Documents",
                newName: "ContextText");
        }
    }
}
