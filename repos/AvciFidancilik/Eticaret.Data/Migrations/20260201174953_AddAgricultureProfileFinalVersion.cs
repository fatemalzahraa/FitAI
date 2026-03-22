using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Eticaret.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAgricultureProfileFinalVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Araziler_Iklimler_IklimId",
                table: "Araziler");

            migrationBuilder.DropForeignKey(
                name: "FK_Araziler_ToprakTipleri_ToprakTipiId",
                table: "Araziler");

            migrationBuilder.DropTable(
                name: "DiseaseRules");

            migrationBuilder.DropTable(
                name: "FidanKdsParametreleri");

            migrationBuilder.DropTable(
                name: "FidanKurallar");

            migrationBuilder.DropTable(
                name: "KdsSecenekler");

            migrationBuilder.DropTable(
                name: "PlantCareGuides");

            migrationBuilder.DropTable(
                name: "ProductTechnicalFeatures");

            migrationBuilder.DropTable(
                name: "Fidanlar");

            migrationBuilder.DropTable(
                name: "Iklimler");

            migrationBuilder.DropTable(
                name: "ToprakTipleri");

            migrationBuilder.DropTable(
                name: "FidanKategoriler");

            migrationBuilder.DropIndex(
                name: "IX_Araziler_IklimId",
                table: "Araziler");

            migrationBuilder.DropIndex(
                name: "IX_Araziler_ToprakTipiId",
                table: "Araziler");

            migrationBuilder.CreateTable(
                name: "ProductAgricultureProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SuitableClimates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuitableSoilTypes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SunlightNeeds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WaterNeeds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AverageYieldPerTreeKg = table.Column<int>(type: "int", nullable: false),
                    HarvestEfficiencyScore = table.Column<int>(type: "int", nullable: false),
                    FirstHarvestYear = table.Column<int>(type: "int", nullable: false),
                    PhMin = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PhMax = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FrostResistant = table.Column<bool>(type: "bit", nullable: false),
                    MinSpacingCm = table.Column<int>(type: "int", nullable: false),
                    MaxSpacingCm = table.Column<int>(type: "int", nullable: false),
                    PlantingDepthCm = table.Column<int>(type: "int", nullable: false),
                    PlantingSeason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IrrigationInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FertilizationInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PruningInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiseaseSymptoms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiagnosisAndSolution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendationReason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAgricultureProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAgricultureProfiles_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductAgricultureProfiles_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAgricultureProfiles_CategoryId",
                table: "ProductAgricultureProfiles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAgricultureProfiles_ProductId",
                table: "ProductAgricultureProfiles",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAgricultureProfiles");

            migrationBuilder.CreateTable(
                name: "DiseaseRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiseaseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IrrigationContext = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Probability = table.Column<int>(type: "int", nullable: false),
                    SolutionAdvice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Symptom = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiseaseRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FidanKategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VarsayilanMaxMesafeCm = table.Column<int>(type: "int", nullable: false),
                    VarsayilanMinMesafeCm = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FidanKategoriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FidanKdsParametreleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgacBasiVerimKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DikimMesafesiM2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FidanKategoriId = table.Column<int>(type: "int", nullable: false),
                    SulamaTipi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TonFiyatTL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToprakTipi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FidanKdsParametreleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Iklimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonRiski = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iklimler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KdsSecenekler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KdsSecenekler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlantCareGuides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FidanKategoriId = table.Column<int>(type: "int", nullable: false),
                    SeasonalAdvice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantCareGuides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTechnicalFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FidanId = table.Column<int>(type: "int", nullable: false),
                    FrostResistant = table.Column<bool>(type: "bit", nullable: false),
                    PhMax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhMin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QualityScore = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTechnicalFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ToprakTipleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuTutmaKatsayisi = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToprakTipleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fidanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FidanKategoriId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BakimRehberi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HastalikBilgisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxDikimMesafesiCm = table.Column<int>(type: "int", nullable: false),
                    MinDikimMesafesiCm = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fidanlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fidanlar_FidanKategoriler_FidanKategoriId",
                        column: x => x.FidanKategoriId,
                        principalTable: "FidanKategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fidanlar_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FidanKurallar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FidanId = table.Column<int>(type: "int", nullable: false),
                    IklimId = table.Column<int>(type: "int", nullable: false),
                    ToprakTipiId = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OnerilenDikimMesafesiCm = table.Column<int>(type: "int", nullable: false),
                    UygunMu = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FidanKurallar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FidanKurallar_Fidanlar_FidanId",
                        column: x => x.FidanId,
                        principalTable: "Fidanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FidanKurallar_Iklimler_IklimId",
                        column: x => x.IklimId,
                        principalTable: "Iklimler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FidanKurallar_ToprakTipleri_ToprakTipiId",
                        column: x => x.ToprakTipiId,
                        principalTable: "ToprakTipleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "IX_Araziler_IklimId",
                table: "Araziler",
                column: "IklimId");

            migrationBuilder.CreateIndex(
                name: "IX_Araziler_ToprakTipiId",
                table: "Araziler",
                column: "ToprakTipiId");

            migrationBuilder.CreateIndex(
                name: "IX_FidanKurallar_FidanId",
                table: "FidanKurallar",
                column: "FidanId");

            migrationBuilder.CreateIndex(
                name: "IX_FidanKurallar_IklimId",
                table: "FidanKurallar",
                column: "IklimId");

            migrationBuilder.CreateIndex(
                name: "IX_FidanKurallar_ToprakTipiId",
                table: "FidanKurallar",
                column: "ToprakTipiId");

            migrationBuilder.CreateIndex(
                name: "IX_Fidanlar_FidanKategoriId",
                table: "Fidanlar",
                column: "FidanKategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Fidanlar_ProductId",
                table: "Fidanlar",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Araziler_Iklimler_IklimId",
                table: "Araziler",
                column: "IklimId",
                principalTable: "Iklimler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Araziler_ToprakTipleri_ToprakTipiId",
                table: "Araziler",
                column: "ToprakTipiId",
                principalTable: "ToprakTipleri",
                principalColumn: "Id");
        }
    }
}
