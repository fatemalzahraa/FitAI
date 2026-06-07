using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitAI.Migrations
{
    /// <inheritdoc />
    public partial class AddTemaToYorum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tema",
                table: "Yorumlar",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tema",
                table: "Yorumlar");
        }
    }
}
