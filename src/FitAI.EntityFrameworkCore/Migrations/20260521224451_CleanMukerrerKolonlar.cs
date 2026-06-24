using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitAI.Migrations
{
    /// <inheritdoc />
    public partial class CleanMukerrerKolonlar : Migration
    {
        /// <inheritdoc />
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1. ÖNCE KOLONA BAĞLI OLAN İNDEKSİ SQL SERVER'DAN SİLİYORUZ (Hatanın kesin çözümü)
    migrationBuilder.DropIndex(
        name: "IX_Yorumlar_UrunId1",
        table: "Yorumlar");

    // 2. ARDINDAN ARTIK ÖZGÜR KALAN MÜKERRER KOLONU SİLİYORUZ
    migrationBuilder.DropColumn(
        name: "UrunId1",
        table: "Yorumlar");

    // Diğer temizlik işlemleriniz (Kullanıcı adı ve Güven Skoru güncellemesi gibi) aşağıda kalabilir:
    migrationBuilder.DropColumn(
        name: "KullaniciAdi",
        table: "Yorumlar");

    migrationBuilder.AlterColumn<double>(
        name: "GuvenSkoru",
        table: "Yorumlar",
        type: "float",
        nullable: true,
        oldClrType: typeof(double),
        oldType: "float");
}        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Yorumlar_Magazalar_MagazaId",
                table: "Yorumlar");

            migrationBuilder.AlterColumn<double>(
                name: "GuvenSkoru",
                table: "Yorumlar",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KullaniciAdi",
                table: "Yorumlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Yorumlar_Magazalar_MagazaId",
                table: "Yorumlar",
                column: "MagazaId",
                principalTable: "Magazalar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
