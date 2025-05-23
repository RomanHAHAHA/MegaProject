using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewsService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "reviews");

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    ProcessedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductSnapshots",
                schema: "reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MainImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSnapshots",
                schema: "reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "reviews",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_Reviews_ProductSnapshots_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "reviews",
                        principalTable: "ProductSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_UserSnapshots_UserId",
                        column: x => x.UserId,
                        principalSchema: "reviews",
                        principalTable: "UserSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                schema: "reviews",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VoteType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => new { x.UserId, x.ReviewId });
                    table.ForeignKey(
                        name: "FK_Votes_Reviews_ReviewUserId_ReviewProductId",
                        columns: x => new { x.ReviewUserId, x.ReviewProductId },
                        principalSchema: "reviews",
                        principalTable: "Reviews",
                        principalColumns: new[] { "UserId", "ProductId" });
                    table.ForeignKey(
                        name: "FK_Votes_UserSnapshots_UserId",
                        column: x => x.UserId,
                        principalSchema: "reviews",
                        principalTable: "UserSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                schema: "reviews",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                schema: "reviews",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ReviewId",
                schema: "reviews",
                table: "Votes",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ReviewUserId_ReviewProductId",
                schema: "reviews",
                table: "Votes",
                columns: new[] { "ReviewUserId", "ReviewProductId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "reviews");

            migrationBuilder.DropTable(
                name: "Votes",
                schema: "reviews");

            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "reviews");

            migrationBuilder.DropTable(
                name: "ProductSnapshots",
                schema: "reviews");

            migrationBuilder.DropTable(
                name: "UserSnapshots",
                schema: "reviews");
        }
    }
}
