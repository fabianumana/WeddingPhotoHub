using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeddingPhotoHub.Migrations
{
    /// <inheritdoc />
    public partial class AddContentTypeToFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsVideo",
                table: "Fotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsVideo",
                table: "Fotos");
        }
    }
}
