using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartsService.Migrations
{
    /// <inheritdoc />
    public partial class RemovedStockQuantityFromProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockQuantity",
                schema: "cart",
                table: "ProductSnapshots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                schema: "cart",
                table: "ProductSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
