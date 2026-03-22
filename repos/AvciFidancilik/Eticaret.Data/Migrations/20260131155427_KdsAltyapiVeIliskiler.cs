using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eticaret.Data.Migrations
{
    /// <inheritdoc />
    public partial class KdsAltyapiVeIliskiler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FidanKategoriId",
                table: "Fidanlar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Fidanlar",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "FidanKategoriler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentScenario",
                table: "ChatLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawData",
                table: "ChatLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StepIndex",
                table: "ChatLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ChatLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ToprakTipi",
                table: "Araziler",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Sehir",
                table: "Araziler",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "IklimId",
                table: "Araziler",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToprakTipiId",
                table: "Araziler",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Araziler",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scenario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StepIndex = table.Column<int>(type: "int", nullable: false),
                    ButtonText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextStepIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatOptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fidanlar_FidanKategoriId",
                table: "Fidanlar",
                column: "FidanKategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Fidanlar_ProductId",
                table: "Fidanlar",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Araziler_IklimId",
                table: "Araziler",
                column: "IklimId");

            migrationBuilder.CreateIndex(
                name: "IX_Araziler_ToprakTipiId",
                table: "Araziler",
                column: "ToprakTipiId");

            migrationBuilder.CreateIndex(
                name: "IX_Araziler_UserId",
                table: "Araziler",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Araziler_AppUsers_UserId",
                table: "Araziler",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Fidanlar_FidanKategoriler_FidanKategoriId",
                table: "Fidanlar",
                column: "FidanKategoriId",
                principalTable: "FidanKategoriler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fidanlar_Products_ProductId",
                table: "Fidanlar",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Araziler_AppUsers_UserId",
                table: "Araziler");

            migrationBuilder.DropForeignKey(
                name: "FK_Araziler_Iklimler_IklimId",
                table: "Araziler");

            migrationBuilder.DropForeignKey(
                name: "FK_Araziler_ToprakTipleri_ToprakTipiId",
                table: "Araziler");

            migrationBuilder.DropForeignKey(
                name: "FK_Fidanlar_FidanKategoriler_FidanKategoriId",
                table: "Fidanlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Fidanlar_Products_ProductId",
                table: "Fidanlar");

            migrationBuilder.DropTable(
                name: "ChatOptions");

            migrationBuilder.DropIndex(
                name: "IX_Fidanlar_FidanKategoriId",
                table: "Fidanlar");

            migrationBuilder.DropIndex(
                name: "IX_Fidanlar_ProductId",
                table: "Fidanlar");

            migrationBuilder.DropIndex(
                name: "IX_Araziler_IklimId",
                table: "Araziler");

            migrationBuilder.DropIndex(
                name: "IX_Araziler_ToprakTipiId",
                table: "Araziler");

            migrationBuilder.DropIndex(
                name: "IX_Araziler_UserId",
                table: "Araziler");

            migrationBuilder.DropColumn(
                name: "FidanKategoriId",
                table: "Fidanlar");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Fidanlar");

            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "FidanKategoriler");

            migrationBuilder.DropColumn(
                name: "CurrentScenario",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "RawData",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "StepIndex",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ChatLogs");

            migrationBuilder.DropColumn(
                name: "IklimId",
                table: "Araziler");

            migrationBuilder.DropColumn(
                name: "ToprakTipiId",
                table: "Araziler");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Araziler");

            migrationBuilder.AlterColumn<string>(
                name: "ToprakTipi",
                table: "Araziler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Sehir",
                table: "Araziler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
