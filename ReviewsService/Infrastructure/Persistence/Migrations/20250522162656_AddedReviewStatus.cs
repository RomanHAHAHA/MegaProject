using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewsService.Migrations
{
    /// <inheritdoc />
    public partial class AddedReviewStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "reviews",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "reviews",
                table: "Reviews");
        }
    }
}
