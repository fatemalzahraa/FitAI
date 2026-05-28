using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitAI.Migrations
{
    /// <inheritdoc />
    public partial class Added_Performance_Indexes_Fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           //// migrationBuilder.DropForeignKey(
           //     name: "FK_Yorumlar_Urunler_UrunId",
           //     table: "Yorumlar");

            migrationBuilder.AddColumn<int>(
                name: "UrunId1",
                table: "Yorumlar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_UrunId1",
                table: "Yorumlar",
                column: "UrunId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Yorumlar_Urunler_UrunId",
                table: "Yorumlar",
                column: "UrunId",
                principalTable: "Urunler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Yorumlar_Urunler_UrunId1",
                table: "Yorumlar",
                column: "UrunId1",
                principalTable: "Urunler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Yorumlar_Urunler_UrunId",
                table: "Yorumlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Yorumlar_Urunler_UrunId1",
                table: "Yorumlar");

            migrationBuilder.DropIndex(
                name: "IX_Yorumlar_UrunId1",
                table: "Yorumlar");

            migrationBuilder.DropColumn(
                name: "UrunId1",
                table: "Yorumlar");

            migrationBuilder.AddForeignKey(
                name: "FK_Yorumlar_Urunler_UrunId",
                table: "Yorumlar",
                column: "UrunId",
                principalTable: "Urunler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
