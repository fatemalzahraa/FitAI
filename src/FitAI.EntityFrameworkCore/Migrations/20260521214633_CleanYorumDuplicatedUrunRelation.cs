using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitAI.Migrations
{
    /// <inheritdoc />
    public partial class CleanYorumDuplicatedUrunRelation : Migration
    {
        /// <inheritdoc />
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Yorumlar tablosundaki mükerrer UrunId1 kolonunu tamamen siler
    migrationBuilder.DropColumn(
        name: "UrunId1",
        table: "Yorumlar");
}
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback durumunda yapılacak işlemler (opsiyonel)
        }
    }
}