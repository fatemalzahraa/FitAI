using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitAI.Migrations
{
    /// <inheritdoc />
    public partial class AddYorumNlpFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Duygu",
                table: "Yorumlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GuvenSkoru",
                table: "Yorumlar",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "KullaniciAdi",
                table: "Yorumlar",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duygu",
                table: "Yorumlar");

            migrationBuilder.DropColumn(
                name: "GuvenSkoru",
                table: "Yorumlar");

            migrationBuilder.DropColumn(
                name: "KullaniciAdi",
                table: "Yorumlar");
        }
    }
}
