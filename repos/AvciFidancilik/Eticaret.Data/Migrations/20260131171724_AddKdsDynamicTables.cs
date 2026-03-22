using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Eticaret.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddKdsDynamicTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FidanKdsParametreleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FidanKategoriId = table.Column<int>(type: "int", nullable: false),
                    ToprakTipi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SulamaTipi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DikimMesafesiM2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AgacBasiVerimKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TonFiyatTL = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FidanKdsParametreleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KdsSecenekler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KdsSecenekler", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FidanKdsParametreleri",
                columns: new[] { "Id", "AgacBasiVerimKg", "DikimMesafesiM2", "FidanKategoriId", "SulamaTipi", "TonFiyatTL", "ToprakTipi" },
                values: new object[,]
                {
                    { 1, 40m, 16.5m, 1, "Su_Modern", 65000m, "Toprak_Tinli" },
                    { 2, 20m, 25m, 1, "Su_Yok", 65000m, "Toprak_Killi" }
                });

            migrationBuilder.InsertData(
                table: "KdsSecenekler",
                columns: new[] { "Id", "Ad", "Kod", "Tip" },
                values: new object[,]
                {
                    { 1, "Damla Sulama (Modern)", "Su_Modern", "Sulama" },
                    { 2, "Salma Sulama (Geleneksel)", "Su_Gelenek", "Sulama" },
                    { 3, "Kuru Tarım (Sadece Yağış)", "Su_Yok", "Sulama" },
                    { 4, "Kumlu (Hafif)", "Toprak_Kumlu", "Toprak" },
                    { 5, "Killi (Ağır)", "Toprak_Killi", "Toprak" },
                    { 6, "Tınlı (İdeal)", "Toprak_Tinli", "Toprak" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FidanKdsParametreleri");

            migrationBuilder.DropTable(
                name: "KdsSecenekler");
        }
    }
}
