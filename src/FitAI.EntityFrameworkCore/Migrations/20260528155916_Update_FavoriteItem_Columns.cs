using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitAI.Migrations
{
    /// <inheritdoc />
    public partial class Update_FavoriteItem_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "FavoriteItems",
                newName: "ProductImage");

            migrationBuilder.AlterColumn<string>(
                name: "Price",
                table: "FavoriteItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "BodyType",
                table: "FavoriteItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyType",
                table: "FavoriteItems");

            migrationBuilder.RenameColumn(
                name: "ProductImage",
                table: "FavoriteItems",
                newName: "ImageUrl");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "FavoriteItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
