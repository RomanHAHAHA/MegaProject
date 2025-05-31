using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartsService.Migrations
{
    /// <inheritdoc />
    public partial class AddedStockQuantityToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                schema: "cart",
                table: "ProductSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockQuantity",
                schema: "cart",
                table: "ProductSnapshots");
        }
    }
}
