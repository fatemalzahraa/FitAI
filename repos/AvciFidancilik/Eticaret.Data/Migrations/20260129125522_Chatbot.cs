using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eticaret.Data.Migrations
{
    /// <inheritdoc />
    public partial class Chatbot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Fidans",
                table: "Fidans");

            migrationBuilder.DropColumn(
                name: "KdsStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "Fidans");

            migrationBuilder.DropColumn(
                name: "DikimAraligiCm",
                table: "Fidans");

            migrationBuilder.DropColumn(
                name: "GunesIhtiyaci",
                table: "Fidans");

            migrationBuilder.DropColumn(
                name: "IklimTipi",
                table: "Fidans");

            migrationBuilder.DropColumn(
                name: "KökDerinligiCm",
                table: "Fidans");

            migrationBuilder.DropColumn(
                name: "LatinceAdi",
                table: "Fidans");

            migrationBuilder.DropColumn(
                name: "MaxSicaklik",
                table: "Fidans");

            migrationBuilder.DropColumn(
                name: "MeyveVerir",
                table: "Fidans");

            migrationBuilder.DropColumn(
                name: "TurAdi",
                table: "Fidans");

            migrationBuilder.RenameTable(
                name: "Fidans",
                newName: "Fidanlar");

            migrationBuilder.RenameColumn(
                name: "ToprakTipi",
                table: "Fidanlar",
                newName: "Ad");

            migrationBuilder.RenameColumn(
                name: "SuIhtiyaci",
                table: "Fidanlar",
                newName: "MinDikimMesafesiCm");

            migrationBuilder.RenameColumn(
                name: "MinSicaklik",
                table: "Fidanlar",
                newName: "MaxDikimMesafesiCm");

            migrationBuilder.AddColumn<string>(
                name: "BakimRehberi",
                table: "Fidanlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HastalikBilgisi",
                table: "Fidanlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fidanlar",
                table: "Fidanlar",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Araziler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Donum = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Sehir = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToprakTipi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Araziler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciMesaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BotCevap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FidanKategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VarsayilanMinMesafeCm = table.Column<int>(type: "int", nullable: false),
                    VarsayilanMaxMesafeCm = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FidanKategoriler", x => x.Id);
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
                name: "ToprakTipleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuTutmaKatsayisi = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToprakTipleri", x => x.Id);
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
                    OnerilenDikimMesafesiCm = table.Column<int>(type: "int", nullable: false),
                    UygunMu = table.Column<bool>(type: "bit", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Araziler");

            migrationBuilder.DropTable(
                name: "ChatLogs");

            migrationBuilder.DropTable(
                name: "FidanKategoriler");

            migrationBuilder.DropTable(
                name: "FidanKurallar");

            migrationBuilder.DropTable(
                name: "Iklimler");

            migrationBuilder.DropTable(
                name: "ToprakTipleri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fidanlar",
                table: "Fidanlar");

            migrationBuilder.DropColumn(
                name: "BakimRehberi",
                table: "Fidanlar");

            migrationBuilder.DropColumn(
                name: "HastalikBilgisi",
                table: "Fidanlar");

            migrationBuilder.RenameTable(
                name: "Fidanlar",
                newName: "Fidans");

            migrationBuilder.RenameColumn(
                name: "MinDikimMesafesiCm",
                table: "Fidans",
                newName: "SuIhtiyaci");

            migrationBuilder.RenameColumn(
                name: "MaxDikimMesafesiCm",
                table: "Fidans",
                newName: "MinSicaklik");

            migrationBuilder.RenameColumn(
                name: "Ad",
                table: "Fidans",
                newName: "ToprakTipi");

            migrationBuilder.AddColumn<int>(
                name: "KdsStatus",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "Fidans",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DikimAraligiCm",
                table: "Fidans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GunesIhtiyaci",
                table: "Fidans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IklimTipi",
                table: "Fidans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "KökDerinligiCm",
                table: "Fidans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LatinceAdi",
                table: "Fidans",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxSicaklik",
                table: "Fidans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MeyveVerir",
                table: "Fidans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TurAdi",
                table: "Fidans",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fidans",
                table: "Fidans",
                column: "Id");
        }
    }
}
