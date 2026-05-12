using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeddingPhotoHub.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFotoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Fotos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Fotos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Fotos");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Fotos");
        }
    }
}
