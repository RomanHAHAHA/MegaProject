using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewsService.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedVoteConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Reviews_ReviewUserId_ReviewProductId",
                schema: "reviews",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_UserSnapshots_UserId",
                schema: "reviews",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                schema: "reviews",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_ReviewId",
                schema: "reviews",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                schema: "reviews",
                table: "Votes");

            migrationBuilder.RenameColumn(
                name: "Rating",
                schema: "reviews",
                table: "Reviews",
                newName: "Rate");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReviewUserId",
                schema: "reviews",
                table: "Votes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ReviewProductId",
                schema: "reviews",
                table: "Votes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                schema: "reviews",
                table: "Votes",
                columns: new[] { "UserId", "ReviewUserId", "ReviewProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Reviews_ReviewUserId_ReviewProductId",
                schema: "reviews",
                table: "Votes",
                columns: new[] { "ReviewUserId", "ReviewProductId" },
                principalSchema: "reviews",
                principalTable: "Reviews",
                principalColumns: new[] { "UserId", "ProductId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Reviews_ReviewUserId_ReviewProductId",
                schema: "reviews",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                schema: "reviews",
                table: "Votes");

            migrationBuilder.RenameColumn(
                name: "Rate",
                schema: "reviews",
                table: "Reviews",
                newName: "Rating");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReviewProductId",
                schema: "reviews",
                table: "Votes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReviewUserId",
                schema: "reviews",
                table: "Votes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewId",
                schema: "reviews",
                table: "Votes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                schema: "reviews",
                table: "Votes",
                columns: new[] { "UserId", "ReviewId" });

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ReviewId",
                schema: "reviews",
                table: "Votes",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Reviews_ReviewUserId_ReviewProductId",
                schema: "reviews",
                table: "Votes",
                columns: new[] { "ReviewUserId", "ReviewProductId" },
                principalSchema: "reviews",
                principalTable: "Reviews",
                principalColumns: new[] { "UserId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_UserSnapshots_UserId",
                schema: "reviews",
                table: "Votes",
                column: "UserId",
                principalSchema: "reviews",
                principalTable: "UserSnapshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
